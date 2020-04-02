using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
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
    ///     xmlns:MyNamespace="clr-namespace:SMS.Client.Controls.Controls.Timelines"
    ///
    ///
    /// 步骤 1b) 在其他项目中存在的 XAML 文件中使用该自定义控件。
    /// 将此 XmlNamespace 特性添加到要使用该特性的标记文件的根
    /// 元素中:
    ///
    ///     xmlns:MyNamespace="clr-namespace:SMS.Client.Controls.Controls.Timelines;assembly=SMS.Client.Controls.Controls.Timelines"
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
    ///     <MyNamespace:TimelineBand/>
    ///
    /// </summary>
    public class TimelineBand : Control
    {
        #region Fields

        private bool _isLoaded = false;
        private bool _isInitialized = false;
        private bool _isTemplateApplied = false;

        private Timeline _hostTimeline = null;
        private Canvas _canvas = null;
        private Point _dragPoint;

        private bool _isDragging = false;
        private List<FrameworkElement> _timeScalesList = new List<FrameworkElement>();
        private List<FrameworkElement> _timeFrameElementList = new List<FrameworkElement>();

        #endregion

        #region Properties

        public Timeline HostTimeline
        {
            get
            {
                return _hostTimeline;
            }

            set
            {
                _hostTimeline = value;
            }
        }

        #endregion

        #region Dependency Properties

        /// <summary>
        /// 是否是主时间条
        /// </summary>
        public static readonly DependencyProperty IsMainBandProperty
            = DependencyProperty.Register("IsMainBand", typeof(bool), typeof(TimelineBand), new PropertyMetadata(false));

        /// <summary>
        /// 当前时间
        /// </summary>
        public static readonly DependencyProperty CurrentDateTimeProperty
            = DependencyProperty.Register("CurrentDateTime", typeof(DateTime), typeof(TimelineBand), new PropertyMetadata(DateTime.Now, new PropertyChangedCallback(OnCurrentDateTimeChanged)));

        /// <summary>
        /// 时间刻度模板
        /// </summary>
        public static readonly DependencyProperty TimeScaleUnitTemplateProperty
            = DependencyProperty.Register("TimeScaleUnitTemplate", typeof(DataTemplate), typeof(TimelineBand), new PropertyMetadata(null));

        /// <summary>
        /// 时间片段模板
        /// </summary>
        public static readonly DependencyProperty TimeFrameTemplateProperty
            = DependencyProperty.Register("TimeFrameTemplate", typeof(DataTemplate), typeof(TimelineBand), new PropertyMetadata(null));

        /// <summary>
        /// 时间条范围最小值
        /// </summary>
        public static readonly DependencyProperty MinDateTimeProperty
            = DependencyProperty.Register("MinDateTime", typeof(DateTime), typeof(TimelineBand), new PropertyMetadata(DateTime.Now.Date));

        /// <summary>
        /// 时间条范围最大值
        /// </summary>
        public static readonly DependencyProperty MaxDateTimeProperty
            = DependencyProperty.Register("MaxDateTime", typeof(DateTime), typeof(TimelineBand), new PropertyMetadata(DateTime.Now.Date.AddDays(1).AddSeconds(-1)));

        /// <summary>
        /// 时间片段数据源
        /// </summary>
        public static readonly DependencyProperty TimeFrameCollectionProperty
            = DependencyProperty.Register("TimeFrameCollection", typeof(ObservableCollection<ITimeRange>), typeof(TimelineBand), new PropertyMetadata(null, OnTimeFrameCollectionChanged));

        /// <summary>
        /// 时间刻度单元宽度
        /// </summary>
        public static readonly DependencyProperty TimeScaleUnitWidthProperty
            = DependencyProperty.Register("TimeScaleUnitWidth", typeof(double), typeof(TimelineBand), new PropertyMetadata(65.0));

        /// <summary>
        /// 时间刻度单元高度
        /// </summary>
        public static readonly DependencyProperty TimeScaleUnitHeightProperty
            = DependencyProperty.Register("TimeScaleUnitHeight", typeof(double), typeof(TimelineBand), new PropertyMetadata(45.0));

        /// <summary>
        /// 时间刻度单元单位
        /// </summary>
        public static readonly DependencyProperty TimeScaleUnitProperty
            = DependencyProperty.Register("TimeScaleUnit", typeof(TimeUnitType), typeof(TimelineBand), new PropertyMetadata(TimeUnitType.Hour));

        #endregion

        #region Dependency Property Wrappers

        public bool IsMainBand
        {
            get { return (bool)GetValue(IsMainBandProperty); }
            set { SetValue(IsMainBandProperty, value); }
        }

        public DateTime CurrentDateTime
        {
            get { return (DateTime)GetValue(CurrentDateTimeProperty); }
            set { SetValue(CurrentDateTimeProperty, value); }
        }

        public DataTemplate TimeScaleUnitTemplate
        {
            get { return (DataTemplate)GetValue(TimeScaleUnitTemplateProperty); }
            set { SetValue(TimeScaleUnitTemplateProperty, value); }
        }

        public DataTemplate TimeFrameTemplate
        {
            get { return (DataTemplate)GetValue(TimeFrameTemplateProperty); }
            set { SetValue(TimeFrameTemplateProperty, value); }
        }

        public DateTime MinDateTime
        {
            get { return (DateTime)GetValue(MinDateTimeProperty); }
            set { SetValue(MinDateTimeProperty, value); }
        }

        public DateTime MaxDateTime
        {
            get { return (DateTime)GetValue(MaxDateTimeProperty); }
            set { SetValue(MaxDateTimeProperty, value); }
        }

        public ObservableCollection<ITimeRange> TimeFrameCollection
        {
            get { return (ObservableCollection<ITimeRange>)GetValue(TimeFrameCollectionProperty); }
            set { SetValue(TimeFrameCollectionProperty, value); }
        }

        public double TimeScaleUnitWidth
        {
            get { return (double)GetValue(TimeScaleUnitWidthProperty); }
            set { SetValue(TimeScaleUnitWidthProperty, value); }
        }

        public double TimeScaleUnitHeight
        {
            get { return (double)GetValue(TimeScaleUnitHeightProperty); }
            set { SetValue(TimeScaleUnitHeightProperty, value); }
        }

        public TimeUnitType TimeScaleUnit
        {
            get { return (TimeUnitType)GetValue(TimeScaleUnitProperty); }
            set { SetValue(TimeScaleUnitProperty, value); }
        }

        #endregion

        #region Routed Events

        public static readonly RoutedEvent TimelineBandStartDragEvent
            = EventManager.RegisterRoutedEvent("TimelineBandStartDrag", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(TimelineBand));

        public static readonly RoutedEvent TimelineBandStopDragEvent
            = EventManager.RegisterRoutedEvent("TimelineBandStopDrag", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(TimelineBand));

        public static readonly RoutedEvent TimelineBandDoubleClickEvent
            = EventManager.RegisterRoutedEvent("TimelineBandDoubleClick", RoutingStrategy.Bubble, typeof(TimelineBandDoubleClickEventHandler), typeof(TimelineBand));

        #endregion

        #region Routed Event Wrappers

        public event RoutedEventHandler TimelineBandStartDrag
        {
            add { AddHandler(TimelineBandStartDragEvent, value); }
            remove { RemoveHandler(TimelineBandStartDragEvent, value); }
        }

        public event RoutedEventHandler TimelineBandStopDrag
        {
            add { AddHandler(TimelineBandStopDragEvent, value); }
            remove { RemoveHandler(TimelineBandStopDragEvent, value); }
        }

        public event TimelineBandDoubleClickEventHandler TimelineBandDoubleClick
        {
            add { AddHandler(TimelineBandDoubleClickEvent, value); }
            remove { RemoveHandler(TimelineBandDoubleClickEvent, value); }
        }

        #endregion

        #region Constructors

        static TimelineBand()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TimelineBand), new FrameworkPropertyMetadata(typeof(TimelineBand)));
        }

        public TimelineBand()
        {
            MouseLeftButtonDown += TimelineBand_MouseLeftButtonDown;

            MouseMove += TimelineBand_MouseMove;
            MouseLeftButtonUp += TimelineBand_MouseLeftButtonUp;

            Loaded += TimelineBand_Loaded;
        }

        #endregion

        #region Override Methods

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _canvas = GetTemplateChild("PART_Canvas") as Canvas;
            if (_canvas != null)
            {
                _canvas.Loaded += _canvas_Loaded;
            }

            _isTemplateApplied = true;
            FirstInitialize();
        }

        #endregion

        #region Dependency Property Changed Callbacks

        private static void OnCurrentDateTimeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TimelineBand timelineBand = d as TimelineBand;
            if (timelineBand == null)
            {
                return;
            }

            DateTime oldDateTime = (DateTime)e.OldValue;
            DateTime newDateTime = (DateTime)e.NewValue;
            timelineBand.UpdateTimelineBandElementsPositionByDateTime(oldDateTime, newDateTime);

            if (timelineBand.HostTimeline != null)
            {
                timelineBand.HostTimeline.CurrentDateTime = timelineBand.CurrentDateTime;
            }
        }

        private static void OnTimeFrameCollectionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TimelineBand timelineBand = d as TimelineBand;
            if (timelineBand == null)
            {
                return;
            }

            ObservableCollection<ITimeRange> oldTimeFrameCollection = e.OldValue as ObservableCollection<ITimeRange>;
            if (oldTimeFrameCollection != null)
            {
                oldTimeFrameCollection.CollectionChanged -= timelineBand.TimeFrameCollection_CollectionChanged;
            }

            ObservableCollection<ITimeRange> newTimeFrameCollection = e.NewValue as ObservableCollection<ITimeRange>;
            if (newTimeFrameCollection != null)
            {
                newTimeFrameCollection.CollectionChanged += timelineBand.TimeFrameCollection_CollectionChanged; ;
            }

            if (timelineBand._isLoaded)
            {
                timelineBand.InitTimeFrameElements();
            }
        }

        #endregion

        #region Event Methods

        private void TimelineBand_Loaded(object sender, RoutedEventArgs e)
        {
            if (TimeFrameCollection == null)
            {
                TimeFrameCollection = new ObservableCollection<ITimeRange>();
            }
        }

        private void _canvas_Loaded(object sender, RoutedEventArgs e)
        {
            _isLoaded = true;
            FirstInitialize();
        }

        private void TimelineBand_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            ReleaseMouseCapture();
            if (_isDragging == true)
            {
                RoutedEventArgs timelineBandStopDragEventArgs = new RoutedEventArgs(TimelineBandStopDragEvent, this);
                RaiseEvent(timelineBandStopDragEventArgs);

                _isDragging = false;
            }
        }

        private void TimelineBand_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _isDragging = true;
            _dragPoint = e.GetPosition(null);

            RoutedEventArgs timelineBandStartDragEventArgs = new RoutedEventArgs(TimelineBandStartDragEvent, this);
            RaiseEvent(timelineBandStartDragEventArgs);

            if (e.ClickCount >= 2)
            {
                Point clickPoint = e.GetPosition(_canvas);
                DateTime clickDateTime = CurrentDateTime.AddTicks((long)((clickPoint.X - _canvas.ActualWidth / 2) / TimeScaleUnitWidth * TimelineHelper.GetScaleUnitTicks(TimeScaleUnit)));
                TimelineBandDoubleClickEventArgs args = new TimelineBandDoubleClickEventArgs(TimelineBandDoubleClickEvent, this, clickDateTime);

                RaiseEvent(args);
            }

            CaptureMouse();
        }

        private void TimelineBand_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                //鼠标水平移动距离
                double moveX = e.GetPosition(null).X - _dragPoint.X;

                long timeScaleUnitTicks = TimelineHelper.GetScaleUnitTicks(TimeScaleUnit);
                long moveTicks = -(long)(moveX / TimeScaleUnitWidth * timeScaleUnitTicks);
                DateTime moveTempDateTime = CurrentDateTime.AddTicks(moveTicks);
                DateTime moveTargetDateTime = moveTempDateTime;
                if (moveTempDateTime < MinDateTime)
                {
                    moveTargetDateTime = MinDateTime;
                }
                else if (moveTempDateTime > MaxDateTime)
                {
                    moveTargetDateTime = MaxDateTime;
                }

                //设置当前时间点
                CurrentDateTime = moveTargetDateTime;
                if (IsMainBand && _hostTimeline != null)
                {
                    _hostTimeline.CurrentDateTime = CurrentDateTime;
                }

                _dragPoint = e.GetPosition(null);
            }
        }

        private void TimeFrameCollection_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            NotifyCollectionChangedAction changedAction = e.Action;
            switch (changedAction)
            {
                case NotifyCollectionChangedAction.Reset:
                    ClearTimeFrameElements();
                    break;
                case NotifyCollectionChangedAction.Add:
                    foreach (object obj in e.NewItems)
                    {
                        ITimeRange timeFrame = obj as ITimeRange;
                        if (timeFrame != null)
                        {
                            AddTimeFrameElement(timeFrame);
                        }
                    }
                    break;
                case NotifyCollectionChangedAction.Remove:
                    foreach (object obj in e.OldItems)
                    {
                        ITimeRange timeFrame = obj as ITimeRange;
                        if (timeFrame != null)
                        {
                            RemoveTimeFrameElement(timeFrame);
                        }
                    }
                    break;
                case NotifyCollectionChangedAction.Replace:
                    if (e.OldItems == null || e.NewItems == null)
                    {
                        break;
                    }

                    int operateCount = Math.Min(e.OldItems.Count, e.NewItems.Count);
                    for (int i = 0; i < operateCount; i++)
                    {
                        ITimeRange oldTimeFrame = e.OldItems[i] as ITimeRange;
                        ITimeRange newTimeFrame = e.NewItems[i] as ITimeRange;
                        if (oldTimeFrame == null || newTimeFrame == null)
                        {
                            continue;
                        }

                        RemoveTimeFrameElement(oldTimeFrame);
                        AddTimeFrameElement(newTimeFrame);
                    }
                    break;
            }
        }

        #endregion

        #region Private Methods

        private void FirstInitialize()
        {
            if (_isInitialized || !_isLoaded || !_isTemplateApplied)
            {
                return;
            }

            InitTimeScales();
            InitTimeFrameElements();
            _isInitialized = true;
        }

        /// <summary>
        /// 初始化时间片段
        /// </summary>
        private void InitTimeFrameElements()
        {
            ClearTimeFrameElements();

            if (TimeFrameCollection == null)
            {
                return;
            }

            foreach (ITimeRange timeFrame in TimeFrameCollection)
            {
                AddTimeFrameElement(timeFrame);
            }
        }

        private void ClearTimeFrameElements()
        {
            foreach (FrameworkElement oldTimeFrame in _timeFrameElementList)
            {
                _canvas.Children.Remove(oldTimeFrame);
            }
            _timeFrameElementList.Clear();
        }

        private void AddTimeFrameElement(ITimeRange timeFrame)
        {
            FrameworkElement timeFrameElement = GetTimeFrameElement(timeFrame);
            _canvas.Children.Add(timeFrameElement);
            _timeFrameElementList.Add(timeFrameElement);
        }

        private void RemoveTimeFrameElement(ITimeRange timeFrame)
        {
            FrameworkElement element = _timeFrameElementList.Find(p => p.DataContext == timeFrame);
            if (element != null)
            {
                _timeFrameElementList.Remove(element);
                _canvas.Children.Remove(element);
            }
        }

        private void InitTimeScales()
        {
            foreach (FrameworkElement oldTimeScale in _timeScalesList)
            {
                _canvas.Children.Remove(oldTimeScale);
            }

            //获取需要绘制的时间刻度的数量
            int scaleUnitCount = (int)Math.Ceiling(_canvas.ActualWidth / TimeScaleUnitWidth) + 4;

            //获取单位时间刻度时长
            long scaleUnitTick = TimelineHelper.GetScaleUnitTicks(TimeScaleUnit);
            long startScaleUnitTicks = CurrentDateTime.Ticks - CurrentDateTime.Ticks % scaleUnitTick - scaleUnitCount / 2 * scaleUnitTick;
            for (int i = 0; i < scaleUnitCount; i++)
            {
                FrameworkElement timeScaleUnit = GetTimeScaleUnit();

                //时间刻度开始点所代表的时间刻度
                DateTime scaleDateTime = new DateTime(startScaleUnitTicks + i * scaleUnitTick);
                double offsetValue = (double)(scaleDateTime - CurrentDateTime).Ticks / scaleUnitTick;
                timeScaleUnit.DataContext = scaleDateTime;
                timeScaleUnit.SetValue(Canvas.LeftProperty, _canvas.ActualWidth / 2 + offsetValue * TimeScaleUnitWidth);

                _canvas.Children.Add(timeScaleUnit);
                _timeScalesList.Add(timeScaleUnit);
            }
        }

        private FrameworkElement GetTimeScaleUnit()
        {
            if (TimeScaleUnitTemplate == null)
            {
                throw new Exception("TimeScaleUnitTemplate不能为null");
            }

            FrameworkElement timeScaleUnit = (TimeScaleUnitTemplate.LoadContent() as FrameworkElement);
            timeScaleUnit.SetValue(Canvas.TopProperty, 0.0);
            timeScaleUnit.SetValue(Panel.ZIndexProperty, 10);
            timeScaleUnit.Width = TimeScaleUnitWidth;
            timeScaleUnit.Height = TimeScaleUnitHeight;

            return timeScaleUnit;
        }

        private FrameworkElement GetTimeFrameElement(ITimeRange timeFrame)
        {
            if (timeFrame == null)
            {
                throw new Exception("TimeFrame不能为null");
            }

            if (TimeFrameTemplate == null)
            {
                throw new Exception("TimeFrameTemplate不能为null");
            }

            double timeScaleUnitTicks = TimelineHelper.GetScaleUnitTicks(TimeScaleUnit);
            FrameworkElement timeFrameElement = TimeFrameTemplate.LoadContent() as FrameworkElement;
            timeFrameElement.DataContext = timeFrame;

            timeFrameElement.Width = (timeFrame.EndTime - timeFrame.BeginTime).Ticks / timeScaleUnitTicks * TimeScaleUnitWidth;
            timeFrameElement.Height = 20;

            double offset = (timeFrame.BeginTime - CurrentDateTime).Ticks / timeScaleUnitTicks * TimeScaleUnitWidth + _canvas.ActualWidth / 2;
            Canvas.SetLeft(timeFrameElement, offset);

            return timeFrameElement;
        }

        private void UpdateTimelineBandElementsPositionByDateTime(DateTime oldDateTime, DateTime newDateTime)
        {
            long timeScaleUnitTicks = TimelineHelper.GetScaleUnitTicks(TimeScaleUnit);

            //TimelineBand元素水平移动距离
            double frameworkElementMoveX = (oldDateTime - newDateTime).Ticks / (double)timeScaleUnitTicks * TimeScaleUnitWidth;

            //移动时间刻度
            MoveTimeScaleUnits(frameworkElementMoveX);

            //移动时间片段
            MoveTimeFrameElements(frameworkElementMoveX);
        }

        /// <summary>
        /// 移动时间刻度
        /// </summary>
        /// <param name="frameworkElementMoveX"></param>
        private void MoveTimeScaleUnits(double frameworkElementMoveX)
        {
            if (_timeScalesList.Count == 0)
            {
                return;
            }

            long timeScaleUnitTicks = TimelineHelper.GetScaleUnitTicks(TimeScaleUnit);
            for (int i = _timeScalesList.Count - 1; i >= 0; i--)
            {
                double scaleUnitLeft = frameworkElementMoveX + Canvas.GetLeft(_timeScalesList[i]);
                Canvas.SetLeft(_timeScalesList[i], scaleUnitLeft);

                //移出超过范围的TimeScaleUnit
                if (scaleUnitLeft < -TimeScaleUnitWidth * 3 || scaleUnitLeft > _canvas.ActualWidth + TimeScaleUnitWidth * 2)
                {
                    _canvas.Children.Remove(_timeScalesList[i]);
                    _timeScalesList.RemoveAt(i);
                    continue;
                }
            }

            FrameworkElement firstScaleElement = _timeScalesList.First();
            int frontAddScaleUnitCount = (int)(Canvas.GetLeft(firstScaleElement) / TimeScaleUnitWidth) + 2;
            for (int i = 0; i < frontAddScaleUnitCount; i++)
            {
                DateTime scaleDateTime = ((DateTime)firstScaleElement.DataContext).AddTicks(-timeScaleUnitTicks);

                FrameworkElement timeScaleUnit = GetTimeScaleUnit();
                timeScaleUnit.DataContext = scaleDateTime;
                timeScaleUnit.SetValue(Canvas.LeftProperty, Canvas.GetLeft(firstScaleElement) - TimeScaleUnitWidth);

                _canvas.Children.Add(timeScaleUnit);
                _timeScalesList.Insert(0, timeScaleUnit);
            }

            int backAddScaleUnitCount = (int)Math.Ceiling((_canvas.ActualWidth - Canvas.GetLeft(_timeScalesList.Last())) / TimeScaleUnitWidth);
            for (int i = 0; i < backAddScaleUnitCount; i++)
            {
                DateTime scaleDateTime = ((DateTime)_timeScalesList.Last().DataContext).AddTicks(timeScaleUnitTicks);

                FrameworkElement timeScaleUnit = GetTimeScaleUnit();
                timeScaleUnit.DataContext = scaleDateTime;
                timeScaleUnit.SetValue(Canvas.LeftProperty, Canvas.GetLeft(_timeScalesList.Last()) + TimeScaleUnitWidth);

                _canvas.Children.Add(timeScaleUnit);
                _timeScalesList.Add(timeScaleUnit);
            }
        }

        /// <summary>
        /// 移动时间片段
        /// </summary>
        /// <param name="frameworkElementMoveX"></param>
        private void MoveTimeFrameElements(double frameworkElementMoveX)
        {
            for (int i = _timeFrameElementList.Count - 1; i >= 0; i--)
            {
                double scaleUnitLeft = frameworkElementMoveX + Canvas.GetLeft(_timeFrameElementList[i]);
                Canvas.SetLeft(_timeFrameElementList[i], scaleUnitLeft);
            }
        }

        #endregion

        #region Public Methods

        #endregion
    }
}
