using GalaSoft.MvvmLight;
using SMS.Client.Common.Utilities;
using SMS.Client.Controls;
using SMS.Client.Host.Models.Tags;
using SMS.StreamMedia.ClientSDK;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Media;

namespace SMS.Client.Host.ViewModels
{
    public class RealtimeMonitorViewModel : ViewModelBase
    {
        #region Fields

        private string _title = "SMS Client";
        private ObservableCollection<ITagModel> _tagCollection = new ObservableCollection<ITagModel>();

        private SpaceTransformer _spaceTransformer = new SpaceTransformer(800, 450);

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

        #endregion

        #region Constructors

        List<Point> points = new List<Point>();
        public RealtimeMonitorViewModel()
        {
            points.Add(new Point(0, 0));
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

            SMClient.Instance.OnGisInfoChanged += Instance_OnGisInfoChanged;
        }

        private void Instance_OnGisInfoChanged(CHCNetSDK.NET_DVR_GIS_UPLOADINFO obj)
        {
            if (obj.fHorizontalValue == 0 || obj.fVerticalValue == 0)
            {
                return;
            }

            for (int i = 0; i < points.Count; i++)
            {
                Point point = points[i];

                Point tagPoint = _spaceTransformer.AngleLocationToScreenLocation(point.X, point.Y, obj.struPtzPos.fPanPos, obj.struPtzPos.fTiltPos, obj.fHorizontalValue, obj.fVerticalValue);
                TagCollection[i].Location = tagPoint;
            }
        }

        #endregion
    }
}
