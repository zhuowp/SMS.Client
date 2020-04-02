using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace SMS.Client.Controls
{
    public class TimelineBandDoubleClickEventArgs : RoutedEventArgs
    {
        #region

        public DateTime ClickDateTime { get; private set; }

        #endregion

        #region Constructors

        public TimelineBandDoubleClickEventArgs(RoutedEvent routedEvent, object source, DateTime clickDateTime) : base(routedEvent, source)
        {
            ClickDateTime = clickDateTime;
        }

        #endregion
    }
}
