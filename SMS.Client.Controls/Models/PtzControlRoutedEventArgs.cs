using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace SMS.Client.Controls
{
    public class PtzControlRoutedEventArgs : RoutedEventArgs
    {
        #region Properties

        public PTZControlType PtzControlType { get; set; }
        public int Speed { get; set; }
        public int StopFlag { get; set; }

        #endregion

        #region Constructors

        public PtzControlRoutedEventArgs() : base()
        { }
        public PtzControlRoutedEventArgs(RoutedEvent routedEvent) : base(routedEvent)
        { }
        public PtzControlRoutedEventArgs(RoutedEvent routedEvent, object source) : base(routedEvent, source)
        { }

        #endregion

    }
}
