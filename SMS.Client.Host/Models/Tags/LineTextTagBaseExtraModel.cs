using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Media;

namespace SMS.Client.Host.Models.Tags
{
    public class LineTextTagBaseExtraModel : ViewModelBase
    {
        #region Fields

        private double _textTagXOffset = 75;
        private double _textTagYOffset = -50;
        private Brush _textTagBackground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#AA6C92FA"));
        private Brush _textTagBorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#6C92FA"));

        #endregion

        #region Properties

        public double TextTagXOffset
        {
            get
            {
                return _textTagXOffset;
            }

            set
            {
                _textTagXOffset = value; RaisePropertyChanged("TextTagXOffset");
            }
        }

        public double TextTagYOffset
        {
            get
            {
                return _textTagYOffset;
            }

            set
            {
                _textTagYOffset = value; RaisePropertyChanged("TextTagYOffset");
            }
        }

        public Brush TextTagBackground
        {
            get
            {
                return _textTagBackground;
            }

            set
            {
                _textTagBackground = value; RaisePropertyChanged("TextTagBackgound");
            }
        }

        public Brush TextTagBorderBrush
        {
            get
            {
                return _textTagBorderBrush;
            }

            set
            {
                _textTagBorderBrush = value; RaisePropertyChanged("TextTagBorderBrush");
            }
        }

        #endregion
    }
}
