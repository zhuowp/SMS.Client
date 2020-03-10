using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace SMS.Client.Controls
{
    public class LineTextTagBase : TagBase
    {
        #region Fields

        private Grid _textTag = null;
        private Line _firstLine = null;
        private Line _secondLine = null;

        private CheckBox _ckbLeft = null;
        private CheckBox _ckbRight = null;

        #endregion

        #region Properties

        internal Grid TextTag
        {
            get
            {
                return _textTag;
            }
        }

        internal Line FirstLine
        {
            get
            {
                return _firstLine;
            }
        }

        internal Line SecondLine
        {
            get
            {
                return _secondLine;
            }
        }

        #endregion

        #region Dependency Properties

        public static readonly DependencyProperty TextTagXOffsetProperty
            = DependencyProperty.Register("TextTagXOffset", typeof(double), typeof(LineTextTagBase), new PropertyMetadata(200.0, OnTextTagXOffsetChanged));

        public static readonly DependencyProperty TextTagYOffsetProperty
            = DependencyProperty.Register("TextTagYOffset", typeof(double), typeof(LineTextTagBase), new PropertyMetadata(-100.0, OnTextTagYOffsetChanged));

        public static readonly DependencyProperty TextTagBackgroundProperty
            = DependencyProperty.Register("TextTagBackground", typeof(Brush), typeof(LineTextTagBase), new PropertyMetadata(new SolidColorBrush(Color.FromArgb(0x66, 0xff, 0x00, 0x00))));

        public static readonly DependencyProperty TextTagBorderBrushProperty
            = DependencyProperty.Register("TextTagBorderBrush", typeof(Brush), typeof(LineTextTagBase), new PropertyMetadata(new SolidColorBrush(Color.FromArgb(0xff, 0xff, 0x00, 0x00))));

        public static readonly DependencyProperty TextTagLocationProperty =
            DependencyProperty.Register("TextTagLocation", typeof(Point), typeof(LineTextTagBase), new PropertyMetadata(new Point(0, 0)));

        #endregion

        #region Dependency Property Wrappers

        /// <summary>
        /// 文字标签框的X向偏移量
        /// </summary>
        public double TextTagXOffset
        {
            get { return (double)GetValue(TextTagXOffsetProperty); }
            set { SetValue(TextTagXOffsetProperty, value); }
        }

        /// <summary>
        /// 文字标签框的Y向偏移量
        /// </summary>
        public double TextTagYOffset
        {
            get { return (double)GetValue(TextTagYOffsetProperty); }
            set { SetValue(TextTagYOffsetProperty, value); }
        }

        /// <summary>
        /// 文字标签框的背景颜色
        /// </summary>
        public Brush TextTagBackground
        {
            get { return (Brush)GetValue(TextTagBackgroundProperty); }
            set { SetValue(TextTagBackgroundProperty, value); }
        }

        /// <summary>
        /// 文字标签框的边框颜色
        /// </summary>
        public Brush TextTagBorderBrush
        {
            get { return (Brush)GetValue(TextTagBorderBrushProperty); }
            set { SetValue(TextTagBorderBrushProperty, value); }
        }

        public Point TextTagLocation
        {
            get { return (Point)GetValue(TextTagLocationProperty); }
            set { SetValue(TextTagLocationProperty, value); }
        }

        #endregion

        #region Constructors

        public LineTextTagBase()
        {
            SizeChanged += LineTextTagBase_SizeChanged;
        }

        #endregion

        #region Private Methods

        private static void OnTextTagXOffsetChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            LineTextTagBase lineTextTag = d as LineTextTagBase;
            lineTextTag.UpdateTextTagLocation();
        }

        private static void OnTextTagYOffsetChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            LineTextTagBase lineTextTag = d as LineTextTagBase;
            lineTextTag.UpdateTextTagLocation();
        }

        private void CheckBox_Click(object sender, RoutedEventArgs e)
        {
        }

        private void LineTextTagBase_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            UpdateTextTagLocation();
        }

        #endregion

        #region Protected Methods

        protected void UpdateTextTagLocation()
        {
            if (FirstLine == null || SecondLine == null)
            {
                return;
            }

            UpdateInnerElementLocationInLineTextTagPanel(); 

            if (IsCheckable && _ckbRight != null && _ckbLeft != null)
            {
                if (TextTagXOffset < 0)
                {
                    _ckbLeft.Visibility = Visibility.Visible;
                    _ckbRight.Visibility = Visibility.Collapsed;
                }
                else
                {
                    _ckbLeft.Visibility = Visibility.Collapsed;
                    _ckbRight.Visibility = Visibility.Visible;
                }
            }
        }
 
        private void UpdateInnerElementLocationInLineTextTagPanel()
        {
            double startOffsetX = 0;
            double startOffsetY = 0;

            FirstLine.X1 = startOffsetX;
            FirstLine.Y1 = startOffsetY;

            FirstLine.X2 = startOffsetX + TextTagXOffset * 2.0 / 3.0;
            FirstLine.Y2 = startOffsetY + TextTagYOffset;

            SecondLine.X1 = FirstLine.X2;
            SecondLine.Y1 = FirstLine.Y2;

            SecondLine.X2 = TextTagXOffset + startOffsetX;
            SecondLine.Y2 = TextTagYOffset + startOffsetY;

            double textTagLeft = SecondLine.X2 + (TextTagXOffset > 0 ? -5 : -TextTag.ActualWidth + 5);
            double textTagTop = SecondLine.Y2 - TextTag.ActualHeight / 2;

            Console.WriteLine(string.Format("SL.X2:{0}, SL.Y2:{1}, TextTagXOffset:{2}, TextTag ActualWidth:{3}, TextTag ActualHeight:{4}",
                SecondLine.X2, SecondLine.Y2, TextTagXOffset, _textTag.ActualWidth, _textTag.ActualHeight));

            Canvas.SetLeft(TextTag, textTagLeft);
            Canvas.SetTop(TextTag, textTagTop);
        }

        #endregion

        #region Public Methods

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _textTag = GetTemplateChild("PART_TextTag") as Grid;
            if (_textTag != null)
            {
                _textTag.SizeChanged += LineTextTagBase_SizeChanged;
            }
             
            _firstLine = GetTemplateChild("PART_Line1") as Line;
            _secondLine = GetTemplateChild("PART_Line2") as Line;

            _ckbLeft = GetTemplateChild("PART_CheckBoxLeft") as CheckBox;
            if (_ckbLeft != null)
            {
                _ckbLeft.Click += CheckBox_Click;
            }

            _ckbRight = GetTemplateChild("PART_CheckBoxRight") as CheckBox;
            if (_ckbRight != null)
            {
                _ckbRight.Click += CheckBox_Click;
            }

            UpdateTextTagLocation();
        }

        #endregion
    }
}
