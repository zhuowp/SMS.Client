using SMS.Client.Common.Models.Tags;
using SMS.Client.Common.Utilities;
using SMS.Client.Controls;
using SMS.Client.MVVM;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows.Media;

namespace SMS.Client.Business.TagConfig.ViewModels
{
    public class TagBaseInfoConfigViewModel : ViewModelBase, ITagBaseInfoConfigViewModel
    {
        #region Fields

        private string _name = string.Empty;
        private TagType _type = TagType.Unknown;
        private TagContentType _contentType = TagContentType.Unknown;
        private ImageSource _icon = null;
        private Color _color;
        private string _description = string.Empty;

        private ObservableCollection<TagType> _tagTypeCollection = new ObservableCollection<TagType>();
        private ObservableCollection<TagContentType> _tagContentTypeCollection = new ObservableCollection<TagContentType>();

        #endregion

        #region Properties

        public string Name
        {
            get
            {
                return _name;
            }

            set
            {
                _name = value; RaisePropertyChanged("Name");
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

        public TagContentType ContentType
        {
            get
            {
                return _contentType;
            }

            set
            {
                _contentType = value; RaisePropertyChanged("ContentType");
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

        public Color Color
        {
            get
            {
                return _color;
            }

            set
            {
                _color = value; RaisePropertyChanged("Color");
            }
        }

        public string Description
        {
            get
            {
                return _description;
            }

            set
            {
                _description = value; RaisePropertyChanged("Description");
            }
        }

        #endregion

        #region Constructors

        public TagBaseInfoConfigViewModel()
        {
            EnumHelper.GetEnumMemberList(typeof(TagType));
        }

        #endregion

        #region Private Methods

        #endregion

        #region Protected Methods

        #endregion

        #region Public Methods

        #endregion
    }
}
