using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms.Integration;

namespace SMS.Client.Controls
{
    public class PlayerBase : Control
    {
        #region Fields

        //视频图像绘制控件
        private System.Windows.Forms.Panel _playScreen = null;
        //承载Winform控件的wpf容器
        private WindowsFormsHost _playScreenWindowsFormsHost = null;

        //播放控件句柄
        private IntPtr _screenHandle = IntPtr.Zero;
        //播放句柄
        private int _playHandle = -1;

        #endregion

        #region Properties

        protected WindowsFormsHost PlayScreenWindowsFormsHost
        {
            set { _playScreenWindowsFormsHost = value; }
        }

        public IntPtr ScreenHandle
        {
            get { return _screenHandle; }
        }

        public int PlayHandle
        {
            get { return _playHandle; }
            protected set { _playHandle = value; }
        }

        public PlayStatus PlayStatus { get; protected set; }

        public int Index { get; set; }

        public object AttachData { get; set; }

        #endregion

        #region Constructors

        static PlayerBase()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(PlayerBase), new FrameworkPropertyMetadata(typeof(PlayerBase)));
        }

        #endregion

        #region Protected Methods

        protected void InitializePlayScreen()
        {
            _playScreen = new System.Windows.Forms.Panel();
            _playScreen.Click += new EventHandler(OnScreenClicked);
            _playScreen.BackColor = System.Drawing.Color.Black;
            _playScreenWindowsFormsHost.Child = _playScreen;
            _screenHandle = _playScreen.Handle;
        }

        protected void DisposePlayScreen()
        {
            _playScreen = null;
            _playScreenWindowsFormsHost.Child = null;
            _screenHandle = IntPtr.Zero;
        }

        #endregion

        #region Private Methods

        private void OnScreenClicked(object sender, EventArgs e)
        {

        }

        #endregion
    }
}
