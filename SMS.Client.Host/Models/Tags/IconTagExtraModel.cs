using GalaSoft.MvvmLight;
using SMS.Client.Controls;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Media;

namespace SMS.Client.Host.Models.Tags
{
    public class IconTagExtraModel : ViewModelBase, ITagExtraModel
    {
        #region Fields

        private double _iconWidth = 44;
        private double _iconHeight = 44;
        private ImageSource _icon = null;
        private ImageSource _mouseOverIcon = null;
        private ImageSource _mouseDownIcon = null;

        #endregion

        #region Properties

        public double IconWidth
        {
            get
            {
                return _iconWidth;
            }

            set
            {
                _iconWidth = value; RaisePropertyChanged("IconWidth");
            }
        }

        public double IconHeight
        {
            get
            {
                return _iconHeight;
            }

            set
            {
                _iconHeight = value; RaisePropertyChanged("IconHeight");
            }
        }

        public ImageSource Icon
        {
            get
            {
                return _icon;
            }

            set
            {
                _icon = value; RaisePropertyChanged("Icon");
            }
        }

        public ImageSource MouseOverIcon
        {
            get
            {
                return _mouseOverIcon;
            }

            set
            {
                _mouseOverIcon = value; RaisePropertyChanged("MouseOverIcon");
            }
        }

        public ImageSource MouseDownIcon
        {
            get
            {
                return _mouseDownIcon;
            }

            set
            {
                _mouseDownIcon = value; RaisePropertyChanged("MouseDownIcon");
            }
        }

        #endregion
    }
}
