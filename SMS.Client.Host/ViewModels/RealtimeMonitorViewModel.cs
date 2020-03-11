using GalaSoft.MvvmLight;
using SMS.Client.Controls;
using SMS.Client.Host.Models.Tags;
using System.Collections.ObjectModel;
using System.Windows;

namespace SMS.Client.Host.ViewModels
{
    public class RealtimeMonitorViewModel : ViewModelBase
    {
        #region Fields

        private string _title = "SMS Client";
        private ObservableCollection<ITagModel> _tagCollection = new ObservableCollection<ITagModel>();

        #endregion

        #region Properties

        public string Title
        {
            get
            {
                return _title;
            }

            set
            {
                _title = value; RaisePropertyChanged("Title");
            }
        }

        public ObservableCollection<ITagModel> TagCollection
        {
            get
            {
                return _tagCollection;
            }

            set
            {
                _tagCollection = value; RaisePropertyChanged("TagCollection");
            }
        }

        #endregion

        #region Constructors

        public RealtimeMonitorViewModel()
        {
            TagModel tagModel = new TagModel()
            {
                Id = "34132",
                TagName = "Test Tag 1",
                Type = TagType.Text,
                Location = new Point(300, 300),
            };
            tagModel.ExtraData = new TextTagExtraModel();

            TagCollection.Add(tagModel);
        }

        #endregion
    }
}
