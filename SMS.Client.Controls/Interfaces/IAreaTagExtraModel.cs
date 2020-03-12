using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows;

namespace SMS.Client.Controls
{
    public interface IAreaTagExtraModel : ITagExtraModel
    {
        ObservableCollection<Point> AreaPoints { get; set; }
    }
}
