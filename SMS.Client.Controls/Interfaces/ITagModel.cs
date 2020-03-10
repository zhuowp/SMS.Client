using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace SMS.Client.Controls
{
    public interface ITagModel
    {
        string Id { get; set; }
        string TagName { get; set; }
        Visibility TagNameVisibility { get; set; }
        Point Location { get; set; }
        bool IsCheckable { get; set; }
        bool IsChecked { get; set; }
        bool IsTagEnabled { get; set; }
        object ExtraData { get; set; }
        TagType Type { get; set; }
        object Data { get; set; }
    }
}
