using System;
using System.Collections.Generic;
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
    ///     <MyNamespace:Timeline/>
    ///
    /// </summary>
    public class Timeline : ContentControl
    {
        #region Fields

        private TimelineBand _mainTimelineBand = null;
        private readonly List<TimelineBand> _allTimelineBand = new List<TimelineBand>();

        #endregion

        #region Properties

        #endregion

        #region Dependency Properties

        public static readonly DependencyProperty CurrentDateTimeProperty =
            DependencyProperty.Register("CurrentDateTime", typeof(DateTime), typeof(Timeline), new PropertyMetadata(DateTime.Now, OnCurrentDateTimeChanged));

        #endregion

        #region Dependency Property Wrappers

        public DateTime CurrentDateTime
        {
            get { return (DateTime)GetValue(CurrentDateTimeProperty); }
            set { SetValue(CurrentDateTimeProperty, value); }
        }

        #endregion

        #region Constructors

        static Timeline()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Timeline), new FrameworkPropertyMetadata(typeof(Timeline)));
        }

        #endregion

        #region Private Methods

        private static void OnCurrentDateTimeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Timeline timeline = d as Timeline;
        }

        private void HookTimelineBands(object element)
        {
            if (element is TimelineBand)
            {
                TimelineBand timelineBand = element as TimelineBand;
                timelineBand.HostTimeline = this;
                if (timelineBand.IsMainBand && _mainTimelineBand == null)
                {
                    _mainTimelineBand = timelineBand;
                }
            }
            else if (element is Panel)
            {
                Panel panel = element as Panel;
                foreach (object childElement in panel.Children)
                {
                    HookTimelineBands(childElement);
                }
            }
        }

        #endregion

        #region Protected Methods

        #endregion

        #region Public Methods

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            HookTimelineBands(Content);
        }

        #endregion
    }
}
