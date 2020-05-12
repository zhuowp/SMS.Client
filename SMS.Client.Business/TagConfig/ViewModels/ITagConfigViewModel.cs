using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace SMS.Client.Business
{
    public interface ITagConfigViewModel
    {
        ICommand ConfirmCommand { get; set; }
    }
}
