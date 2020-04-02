using SMS.Client.Business;
using SMS.Client.Controls;
using SMS.Client.Host.Helpers;
using SMS.Client.Host.Models;
using SMS.Client.Host.ViewModels;
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
            //realtimeMonitorView.DataContext = new RealtimeMonitorViewModel();

            //realtimeMonitorView.CloseWindow += RealtimeMonitorView_CloseWindow;
            //Loaded += MainWindow_Loaded;
            //Closed += MainWindow_Closed;
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
            if (realtimeMonitorView.TagContainer != null)
            {
                realtimeMonitorView.TagContainer.TagPreviewMouseRightButtonUp += TagContainer_TagPreviewMouseRightButtonUp; ;
                realtimeMonitorView.TagContainer.TagPreviewMouseLeftButtonDown += TagContainer_TagPreviewMouseLeftButtonDown; ;
                realtimeMonitorView.TagContainer.TagPreviewMouseLeftButtonUp += TagContainer_TagPreviewMouseLeftButtonUp; ;
                realtimeMonitorView.TagContainer.TagPreviewMouseMove += TagContainer_TagPreviewMouseMove; ;
                realtimeMonitorView.TagContainer.TagClick += TagContainer_TagClick; ;
                realtimeMonitorView.TagContainer.TagLocationChanged += TagContainer_TagLocationChanged; ;
            }

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

        private void TagContainer_TagLocationChanged(TagBase arg1, ITagModel arg2)
        {
        }

        private void TagContainer_TagClick(TagBase arg1, RoutedEventArgs arg2)
        {
            RealtimePlayWindow realPlayWindow = new RealtimePlayWindow(400, 300);

            realPlayWindow.Loaded += (s, e) =>
            {
                VideoPlayModel playModel = new VideoPlayModel();

                playModel.Ip = "192.168.28.136";
                playModel.Port = 8000;
                playModel.Channel = 1;
                playModel.UserName = "admin";
                playModel.Password = "admin12345";
                playModel.StreamType = 0;

                realPlayWindow.RealPlayer.PlayHelper = new VideoPlayHelper();
                Task.Factory.StartNew(() =>
                {
                    Thread.Sleep(1000);
                    Dispatcher.Invoke(() =>
                    {
                        realPlayWindow.RealPlayer.StartPlay(playModel);
                    });
                });
            };

            realPlayWindow.Show();
        }

        private void TagContainer_TagPreviewMouseMove(TagBase arg1, MouseEventArgs arg2)
        {
        }

        private void TagContainer_TagPreviewMouseLeftButtonUp(TagBase arg1, MouseButtonEventArgs arg2)
        {
        }

        private void TagContainer_TagPreviewMouseLeftButtonDown(TagBase arg1, MouseButtonEventArgs arg2)
        {
        }

        private void TagContainer_TagPreviewMouseRightButtonUp(TagBase arg1, MouseButtonEventArgs arg2)
        {
        }
    }
}
