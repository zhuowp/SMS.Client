using GalaSoft.MvvmLight;
using SMS.Client.Controls;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace SMS.Client.Host.Models.Tags
{
    public class TagModel : ViewModelBase, ITagModel
    {
        #region Fields

        private string _id = string.Empty;
        private string _tagName = string.Empty;
        private TagType _type = TagType.Unknown;
        private Visibility _tagNameVisibility = Visibility.Visible;
        private Point _location = new Point();
        private bool _isCheckable = true;
        private bool _isChecked = false;
        private bool _isTagEnabled = true;
        private object _extraData = null;
        private object _data = null;

        #endregion

        #region Properties

        public string Id
        {
            get
            {
                return _id;
            }

            set
            {
                _id = value; RaisePropertyChanged("Id");
            }
        }

        public string TagName
        {
            get
            {
                return _tagName;
            }

            set
            {
                _tagName = value; RaisePropertyChanged("TagName");
            }
        }

        public TagType Type
        {
            get
            {
                return _type;
            }

            set
            {
                _type = value; RaisePropertyChanged("Type");
            }
        }

        public Visibility TagNameVisibility
        {
            get
            {
                return _tagNameVisibility;
            }

            set
            {
                _tagNameVisibility = value; RaisePropertyChanged("TagNameVisibility");
            }
        }

        public Point Location
        {
            get
            {
                return _location;
            }

            set
            {
                _location = value; RaisePropertyChanged("Location");
            }
        }

        public bool IsCheckable
        {
            get
            {
                return _isCheckable;
            }

            set
            {
                _isCheckable = value; RaisePropertyChanged("IsCheckable");
            }
        }

        public bool IsChecked
        {
            get
            {
                return _isChecked;
            }

            set
            {
                _isChecked = value; RaisePropertyChanged("IsChecked");
            }
        }

        public bool IsTagEnabled
        {
            get
            {
                return _isTagEnabled;
            }

            set
            {
                _isTagEnabled = value; RaisePropertyChanged("IsTagEnabled");
            }
        }

        public object ExtraData
        {
            get
            {
                return _extraData;
            }

            set
            {
                _extraData = value; RaisePropertyChanged("ExtraData");
            }
        }

        public object Data
        {
            get
            {
                return _data;
            }

            set
            {
                _data = value; RaisePropertyChanged("Data");
            }
        }

        #endregion
    }
}
