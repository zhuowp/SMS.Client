using SMS.Client.Host.Helpers;
using SMS.Client.Host.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

            Loaded += MainWindow_Loaded;
            Closed += MainWindow_Closed;
        }

        private void MainWindow_Closed(object sender, EventArgs e)
        {
            realtimeMonitorView.Player.StopPlay();
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            VideoPlayModel playModel = new VideoPlayModel();

            playModel.Ip = "192.168.28.130";
            playModel.Port = 8000;
            playModel.Channel = 1;
            playModel.UserName = "admin";
            playModel.Password = "admin888";
            playModel.StreamType = 0;

            realtimeMonitorView.Player.PlayHelper = new VideoPlayHelper();
            realtimeMonitorView.Player?.StartPlay(playModel);
        }
    }
}
