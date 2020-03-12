using SMS.Client.Log;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SMS.Client.Controls
{
    /// <summary>
    /// 按照步骤 1a 或 1b 操作，然后执行步骤 2 以在 XAML 文件中使用此自定义控件。
    ///
    /// 步骤 1a) 在当前项目中存在的 XAML 文件中使用该自定义控件。
    /// 将此 XmlNamespace 特性添加到要使用该特性的标记文件的根
    /// 元素中:
    ///
    ///     xmlns:MyNamespace="clr-namespace:SMS.Client.Controls.Controls.Tags"
    ///
    ///
    /// 步骤 1b) 在其他项目中存在的 XAML 文件中使用该自定义控件。
    /// 将此 XmlNamespace 特性添加到要使用该特性的标记文件的根
    /// 元素中:
    ///
    ///     xmlns:MyNamespace="clr-namespace:SMS.Client.Controls.Controls.Tags;assembly=SMS.Client.Controls.Controls.Tags"
    ///
    /// 您还需要添加一个从 XAML 文件所在的项目到此项目的项目引用，
    /// 并重新生成以避免编译错误:
    ///
    ///     在解决方案资源管理器中右击目标项目，然后依次单击
    ///     “添加引用”->“项目”->[浏览查找并选择此项目]
    ///
    ///
    /// 步骤 2)
    /// 继续操作并在 XAML 文件中使用控件。
    ///
    ///     <MyNamespace:TagContainer/>
    ///
    /// </summary>
    public class TagContainer : Control
    {
        #region Fields

        private Canvas _canvasTags = null;
        private bool _isTagRefreshThreadInit = false;
        private bool _isInitializeTags = false;
        private bool _isResetTags = false;
        private ObservableCollection<ITagModel> _toInitializeTags = null;
        private readonly List<ITagModel> _addTagList = new List<ITagModel>();
        private readonly List<ITagModel> _updateTagList = new List<ITagModel>();
        private readonly List<ITagModel> _removeTagList = new List<ITagModel>();
        private readonly ConcurrentDictionary<string, TagBase> _tagCacheDict = new ConcurrentDictionary<string, TagBase>();
        private readonly object _lock = new object();

        public event Action<TagBase, MouseButtonEventArgs> TagPreviewMouseRightButtonUp;
        public event Action<TagBase, MouseButtonEventArgs> TagPreviewMouseLeftButtonDown;
        public event Action<TagBase, MouseButtonEventArgs> TagPreviewMouseLeftButtonUp;
        public event Action<TagBase, MouseEventArgs> TagPreviewMouseMove;
        public event Action<TagBase, RoutedEventArgs> TagClick;
        public event Action<TagBase, ITagModel> TagLocationChanged;

        #endregion

        #region Properties

        #endregion

        #region Dependency Properties

        public static readonly DependencyProperty TagItemsSourceProperty =
            DependencyProperty.Register("TagItemsSource", typeof(ObservableCollection<ITagModel>), typeof(TagContainer), new PropertyMetadata(null, OnTagItemsSourceChanged));

        #endregion

        #region Dependency Property Wrappers

        public ObservableCollection<ITagModel> TagItemsSource
        {
            get { return (ObservableCollection<ITagModel>)GetValue(TagItemsSourceProperty); }
            set { SetValue(TagItemsSourceProperty, value); }
        }

        #endregion

        #region Routed Events

        public static readonly RoutedEvent TagsRefreshedEvent =
            EventManager.RegisterRoutedEvent("TagsRefreshed", RoutingStrategy.Bubble, typeof(EventHandler<RoutedEventArgs>), typeof(TagContainer));

        #endregion

        #region Routed Event Wrappers

        public event RoutedEventHandler TagsRefreshed
        {
            add { AddHandler(TagsRefreshedEvent, value); }
            remove { RemoveHandler(TagsRefreshedEvent, value); }
        }

        #endregion

        #region Constructors

        static TagContainer()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TagContainer), new FrameworkPropertyMetadata(typeof(TagContainer)));
        }

        public TagContainer()
        {
            Loaded += TagContainer_Loaded;
        }

        #endregion

        #region Private Methods

        private static void OnTagItemsSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TagContainer tagContainer = d as TagContainer;

            //注销旧集合更新通知事件
            if (e.OldValue is ObservableCollection<ITagModel> oldTagsSource)
            {
                oldTagsSource.CollectionChanged -= tagContainer.TagsSource_CollectionChanged;
            }

            //注册新集合更新通知事件
            ObservableCollection<ITagModel> newTagsSource = e.NewValue as ObservableCollection<ITagModel>;
            tagContainer.GetReadyToInitializeTagsDisplay(newTagsSource);
            if (newTagsSource != null)
            {
                newTagsSource.CollectionChanged += tagContainer.TagsSource_CollectionChanged;
            }
        }

        private void TagContainer_Loaded(object sender, RoutedEventArgs e)
        {
            if (!_isTagRefreshThreadInit)
            {
                _isTagRefreshThreadInit = true;
                Task.Factory.StartNew(ContinuousRefreshTags, TaskCreationOptions.LongRunning);
            }
        }

        private void TagsSource_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            e.OnNotifyCollectionChanged<ITagModel>(
                p => _addTagList.Add(p),
                p => _removeTagList.Add(p),
                (newItem, oldItem) => _updateTagList.Add(newItem),
                () => _isResetTags = true);
        }

        #region Tags Refresh

        private void GetReadyToInitializeTagsDisplay(ObservableCollection<ITagModel> tagModels)
        {
            _toInitializeTags = tagModels;

            //清空更新（增删改）标签列表
            _addTagList.Clear();
            _updateTagList.Clear();
            _removeTagList.Clear();

            _isInitializeTags = true;
        }

        private void ContinuousRefreshTags()
        {
            while (true)
            {
                try
                {
                    Dispatcher.Invoke(() =>
                    {
                        TryRefreshTagsDisplay();
                    });
                }
                catch (Exception ex)
                {
                    LogHelper.DebugFormatted("标签刷新异常，异常信息：{0}", ex.Message);
                }
                finally
                {
                    Thread.Sleep(200);
                }
            }
        }

        private bool TryRefreshTagsDisplay()
        {
            //重置所有标签
            bool resetResult = TryResetTagsDisplay();

            //初始化所有标签
            bool initResult = TryInitializeTagsDisplay(_toInitializeTags);

            //新增所有指定标签
            bool addResult = TryAddTags();

            //更新所有指定标签
            bool updateResult = TryUpdateTags();

            //移除所有指定标签
            bool removeResult = TryRemoveTags();

            return resetResult || initResult || addResult || updateResult || removeResult;
        }

        private bool TryResetTagsDisplay()
        {
            bool result = false;

            if (_isResetTags)
            {
                _isResetTags = false;
                ClearAllTags(_tagCacheDict);
            }

            return result;
        }

        private bool TryInitializeTagsDisplay(IList<ITagModel> tagModels)
        {
            if (!_isInitializeTags)
            {
                return false;
            }

            _isInitializeTags = false;
            ClearAllTags(_tagCacheDict);

            if (tagModels != null)
            {
                foreach (ITagModel tagModel in tagModels)
                {
                    AddTagDisplay(_tagCacheDict, tagModel);
                }
            }

            return true;
        }

        private bool TryAddTags()
        {
            bool result = false;
            if (_addTagList.Count != 0)
            {
                foreach (ITagModel tagModel in _addTagList)
                {
                    AddTagDisplay(_tagCacheDict, tagModel);
                }

                _addTagList.Clear();
            }

            return result;
        }

        private bool TryUpdateTags()
        {
            bool result = false;
            if (_updateTagList.Count != 0)
            {
                foreach (ITagModel tagModel in _updateTagList)
                {
                    UpdateTagDisplay(_tagCacheDict, tagModel);
                }
                _updateTagList.Clear();
            }

            return result;
        }

        private bool TryRemoveTags()
        {
            bool result = false;
            if (_removeTagList.Count != 0)
            {
                foreach (ITagModel tagModel in _removeTagList)
                {
                    RemoveTagDisplay(_tagCacheDict, tagModel); ;
                }
                _removeTagList.Clear();
            }

            return result;
        }

        private void ClearAllTags(ConcurrentDictionary<string, TagBase> tagCacheDict)
        {
            //所有标签从界面中移除
            foreach (var existTag in tagCacheDict.Values)
            {
                _canvasTags.Children.Remove(existTag);
                if (existTag.DataContext is ITagModel tagModel)
                {
                    tagModel.PropertyChanged -= TagModel_PropertyChanged;
                }
            }

            //清空缓存数据
            tagCacheDict.Clear();
        }

        private void AddTagDisplay(ConcurrentDictionary<string, TagBase> tagCacheDict, ITagModel addTagModel)
        {
            addTagModel.PropertyChanged += TagModel_PropertyChanged;
            TagBase tag = TagFactory.Instance.Create(addTagModel);

            tag.Click += Tag_Click;
            tag.PreviewMouseRightButtonUp += Tag_PreviewMouseRightButtonUp;

            tag.PreviewMouseLeftButtonDown += Tag_PreviewMouseLeftButtonDown;
            tag.PreviewMouseMove += Tag_PreviewMouseMove;
            tag.PreviewMouseLeftButtonUp += Tag_PreviewMouseLeftButtonUp;

            _canvasTags.Children.Add(tag);
            tagCacheDict.AddOrUpdate(addTagModel.Id, tag, (k, v) => tag);
        }

        private void UpdateTagDisplay(ConcurrentDictionary<string, TagBase> tagCacheDict, ITagModel updateTagModel)
        {
            if (tagCacheDict.TryGetValue(updateTagModel.Id, out TagBase tagBase))
            {
                if (tagBase.DataContext is ITagModel oldTagModel)
                {
                    oldTagModel.PropertyChanged -= TagModel_PropertyChanged;
                }

                tagBase.DataContext = updateTagModel;
                updateTagModel.PropertyChanged += TagModel_PropertyChanged;
            }
        }

        private void RemoveTagDisplay(ConcurrentDictionary<string, TagBase> tagCacheDict, ITagModel removeTagModel)
        {
            if (tagCacheDict.TryRemove(removeTagModel.Id, out TagBase tagBase))
            {
                tagBase.Click -= Tag_Click;
                tagBase.PreviewMouseRightButtonUp -= Tag_PreviewMouseRightButtonUp;

                tagBase.PreviewMouseLeftButtonDown -= Tag_PreviewMouseLeftButtonDown;
                tagBase.PreviewMouseMove -= Tag_PreviewMouseMove;
                tagBase.PreviewMouseLeftButtonUp -= Tag_PreviewMouseLeftButtonUp;

                _canvasTags.Children.Remove(tagBase);

                if (tagBase.DataContext is ITagModel tagModel)
                {
                    tagModel.PropertyChanged -= TagModel_PropertyChanged;
                }
            }
        }

        #endregion

        #region Tags Event

        private void Tag_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            TagPreviewMouseLeftButtonUp?.Invoke((TagBase)sender, e);
        }

        private void Tag_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            TagPreviewMouseMove?.Invoke((TagBase)sender, e);
        }

        private void Tag_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            TagPreviewMouseLeftButtonDown?.Invoke((TagBase)sender, e);
        }

        private void Tag_PreviewMouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            TagPreviewMouseRightButtonUp?.Invoke((TagBase)sender, e);
        }

        private void Tag_Click(object sender, RoutedEventArgs e)
        {
            TagClick?.Invoke((TagBase)sender, e);
        }

        private void TagModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Location")
            {
                ITagModel tagModel = sender as ITagModel;

                if (_tagCacheDict.TryGetValue(tagModel.Id, out TagBase tagBase))
                {
                    TagLocationChanged?.Invoke(tagBase, tagModel);
                }
            }
        }

        #endregion

        #endregion

        #region Protected Methods

        #endregion

        #region Public Methods

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _canvasTags = GetTemplateChild("PART_TagCanvas") as Canvas;
        }

        #endregion

    }
}
