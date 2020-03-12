using SMS.Client.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows;
using System.Windows.Media;

namespace SMS.Client.Host.Models.Tags
{
    public class AreaTagExtraModel : LineTextTagBaseExtraModel, IAreaTagExtraModel
    {
        #region Fields

        private ObservableCollection<Point> _areaPoints = new ObservableCollection<Point>();
        private Color _areaColor = (Color)ColorConverter.ConvertFromString("#6C92FA");

        #endregion

        #region Properties

        public ObservableCollection<Point> AreaPoints
        {
            get
            {
                return _areaPoints;
            }

            set
            {
                _areaPoints = value; RaisePropertyChanged("AreaPoints");
            }
        }

        public Color AreaColor
        {
            get { return _areaColor; }
            set { _areaColor = value; RaisePropertyChanged("AreaPoints"); }
        }

        #endregion
    }
}
