using SMS.Client.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace SMS.Client.Business
{
    public interface IRealtimeMonitorViewModel
    {
        string Title { get; set; }
        ObservableCollection<ITagModel> TagCollection { get; set; }
    }
}
