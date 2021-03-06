﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms.Integration;

namespace SMS.Client.Controls
{
    public class PlayerBase : ContentControl
    {
        #region Fields

        //视频图像绘制控件
        private System.Windows.Forms.Panel _playScreen = null;
        //承载Winform控件的wpf容器
        private WindowsFormsHost _playScreenWindowsFormsHost = null;

        #endregion

        #region Properties

        protected WindowsFormsHost PlayScreenWindowsFormsHost
        {
            set { _playScreenWindowsFormsHost = value; }
        }

        public IntPtr ScreenHandle { get; private set; } = IntPtr.Zero;

        public long PlayHandle { get; protected set; } = -1;

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
            ScreenHandle = _playScreen.Handle;
        }

        protected void DisposePlayScreen()
        {
            _playScreen = null;
            _playScreenWindowsFormsHost.Child = null;
            ScreenHandle = IntPtr.Zero;
        }

        #endregion

        #region Private Methods

        private void OnScreenClicked(object sender, EventArgs e)
        {
            
        }

        #endregion
    }
}
