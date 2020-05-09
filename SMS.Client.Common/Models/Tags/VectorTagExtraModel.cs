using SMS.Client.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows;
using System.Windows.Media;

namespace SMS.Client.Common.Models.Tags
{
    public class VectorTagExtraModel : LineTextTagBaseExtraModel, IAreaTagExtraModel
    {
        #region Fields

        private ObservableCollection<Point> _areaPoints = new ObservableCollection<Point>();
        private Brush _arrowBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#886C92FA"));

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

        public Brush ArrowBrush
        {
            get
            {
                return _arrowBrush;
            }

            set
            {
                _arrowBrush = value; RaisePropertyChanged("ArrowBrush");
            }
        }

        #endregion
    }
}
