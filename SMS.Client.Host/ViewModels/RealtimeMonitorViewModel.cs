using GalaSoft.MvvmLight;

namespace SMS.Client.Host.ViewModels
{
    public class RealtimeMonitorViewModel : ViewModelBase
    {
        #region Fields

        private string _title = "SMS Client";

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

        #endregion

        #region Constructors

        public RealtimeMonitorViewModel()
        {

        }

        #endregion
    }
}
