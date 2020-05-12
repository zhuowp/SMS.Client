using SMS.Client.MVVM;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace SMS.Client.Business
{
    public class TagConfigViewModel : ViewModelBase, ITagConfigViewModel
    {
        #region Fields

        #endregion

        #region Properties

        public ICommand ConfirmCommand { get; set; }

        #endregion

        #region Constructors

        public TagConfigViewModel()
        {
            InitCommands();
        }

        #endregion

        #region Private Methods

        private void InitCommands()
        {
            ConfirmCommand = new RelayCommand(SaveTagInfo);
        }

        private void SaveTagInfo()
        {
        }

        #endregion

        #region Protected Methods

        #endregion

        #region Public Methods

        #endregion
    }
}
