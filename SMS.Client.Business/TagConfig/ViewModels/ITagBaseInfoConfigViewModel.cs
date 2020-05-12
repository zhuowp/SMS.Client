using SMS.Client.Common.Models.Tags;
using SMS.Client.Controls;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Media;

namespace SMS.Client.Business.TagConfig.ViewModels
{
    public interface ITagBaseInfoConfigViewModel
    {
        string Name { get; set; }
        TagType Type { get; set; }
        TagContentType ContentType { get; set; }
        ImageSource Icon { get; set; }
        Color Color { get; set; }
        string Description { get; set; }
    }
}
