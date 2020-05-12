using SMS.Client.Business;
using SMS.Client.Common.Models;
using SMS.Client.Common.Utilities;
using SMS.Client.Controls;
using SMS.Client.Log;
using SMS.StreamMedia.ClientSDK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SMS.Client
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            realtimeMonitorView.DataContext = new RealtimeMonitorViewModel();
            realtimeMonitorView.CloseWindow += RealtimeMonitorView_CloseWindow;

            Loaded += MainWindow_Loaded;
            Closed += MainWindow_Closed;
        }

        private void RealtimeMonitorView_CloseWindow()
        {
            realtimeMonitorView.Dispose();
            Close();
        }

        private void MainWindow_Closed(object sender, EventArgs e)
        {
            realtimeMonitorView.Player.StopPlay();
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            VideoPlayModel playModel = new VideoPlayModel
            {
                Ip = "192.168.28.136",
                Port = 8000,
                Channel = 1,
                UserName = "admin",
                Password = "admin12345",
                StreamType = 0
            };

            realtimeMonitorView.Player?.StartPlay(playModel);
        }

    }
}
