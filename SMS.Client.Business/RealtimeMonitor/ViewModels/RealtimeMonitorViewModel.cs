using SMS.Client.Common.Caches;
using SMS.Client.Common.Models;
using SMS.Client.Common.Models.Tags;
using SMS.Client.Common.Utilities;
using SMS.Client.Controls;
using SMS.Client.MVVM;
using SMS.StreamMedia.ClientSDK;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace SMS.Client.Business
{
    public class RealtimeMonitorViewModel : ViewModelBase, IRealtimeMonitorViewModel
    {
        #region Fields

        private string _title = "SMS Client";
        private ObservableCollection<ITagModel> _tagCollection = new ObservableCollection<ITagModel>();

        private SpaceTransformer _spaceTransformer = new SpaceTransformer(800, 600);

        #endregion

        #region Properties

        public string Title
        {
            get
            {
                return _title;
            }

            set
            {
                _title = value; RaisePropertyChanged("Title");
            }
        }

        public ObservableCollection<ITagModel> TagCollection
        {
            get
            {
                return _tagCollection;
            }

            set
            {
                _tagCollection = value; RaisePropertyChanged("TagCollection");
            }
        }

        public ICommand AddTagCommand { get; set; }

        #endregion

        #region Constructors

        List<Point> points = new List<Point>();
        public RealtimeMonitorViewModel()
        {
            points.Add(new Point(33, 44));
            points.Add(new Point(245, 28));
            //points.Add(new Point(90, 20));
            //points.Add(new Point(180, -30));

            TagModel tagModel1 = new TagModel()
            {
                Id = "34132",
                TagName = "Test Text Tag 1",
                Type = TagType.Text,
                Location = new Point(300, 300),
                ExtraData = new TextTagExtraModel()
                {
                    TextTagBorderBrush = Brushes.AliceBlue
                }
            };
            TagCollection.Add(tagModel1);

            TagModel tagModel2 = new TagModel()
            {
                Id = "3ew41",
                TagName = "Test Icon Tag 1",
                Type = TagType.Icon,
                Location = new Point(400, 300),
                ExtraData = new IconTagExtraModel()
                {
                    Icon = Application.Current.Resources["box_camera_blue"] as ImageSource,
                }
            };
            TagCollection.Add(tagModel2);

            //TagModel tagModel3 = new TagModel()
            //{
            //    Id = "342124",
            //    TagName = "Test Area Tag 1",
            //    Type = TagType.Area,
            //    Location = new Point(500, 300),
            //    ExtraData = new AreaTagExtraModel()
            //    {
            //        AreaPoints = new ObservableCollection<Point>() { new Point(500, 300), new Point(500, 350), new Point(400, 330) }
            //    },
            //};
            //TagCollection.Add(tagModel3);

            //TagModel tagModel4 = new TagModel()
            //{
            //    Id = "reqewuip",
            //    TagName = "Test Vector Tag 1",
            //    Type = TagType.Vector,
            //    Location = new Point(200, 300),
            //    ExtraData = new VectorTagExtraModel()
            //    {
            //        AreaPoints = new ObservableCollection<Point>() { new Point(200, 200), new Point(500, 200) }
            //    },
            //};
            //TagCollection.Add(tagModel4);

            HolographicInfoCache.Instance.OnHolographicInfoChanged += Instance_OnHolographicInfoChanged;
            InitCommands();
        }

        #endregion

        #region Private Methods

        private void InitCommands()
        {
            AddTagCommand = new RelayCommand<object>(ShowAddTagWindow);
        }

        private void Instance_OnHolographicInfoChanged(HolographicInfo obj)
        {
            for (int i = 0; i < points.Count; i++)
            {
                Point point = points[i];
                //_spaceTransformer.Pt2Xy(obj.fHorizontalValue, obj.fVerticalValue, obj.struPtzPos.fPanPos, obj.struPtzPos.fTiltPos, point.X, point.Y, out double x, out double y);

                Point tagPoint = _spaceTransformer.AngleLocationToScreenLocation(point.X, point.Y, obj.CameraParameter);
                TagCollection[i].Location = tagPoint;
            }
        }

        private void ShowAddTagWindow(object obj)
        {
        }

        #endregion
    }
}
