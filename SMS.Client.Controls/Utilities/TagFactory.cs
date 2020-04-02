using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Data;

namespace SMS.Client.Controls
{
    public class TagFactory
    {
        #region Fields

        private static TagFactory _instance = null;
        private static object _lock = new object();

        #endregion

        #region Properties

        public static TagFactory Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        if (_instance == null)
                        {
                            _instance = new TagFactory();
                        }
                    }
                }

                return _instance;
            }
        }

        #endregion

        #region Constructors

        private TagFactory()
        { }

        #endregion

        #region Private Methods

        private IconTag CreateIconTag(ITagModel tagModel)
        {
            tagModel.TagNameVisibility = Visibility.Collapsed;
            IconTag tag = new IconTag();

            SetTagBasePropertyBinding(tag);
            SetIconTagPropertyBindings(tag);

            return tag;
        }

        private LineTextTag CreateTextTag(ITagModel tagModel)
        {
            LineTextTag tag = new LineTextTag();
            SetTagBasePropertyBinding(tag);
            SetLineTextTagBasePropertyBindings(tag);
            SetTextTagPropertyBindings(tag);

            return tag;
        }

        private AreaTag CreateAreaTag(ITagModel tagModel)
        {
            AreaTag tag = new AreaTag();
            SetTagBasePropertyBinding(tag);
            SetLineTextTagBasePropertyBindings(tag);
            SetAreaTagPropertyBindings(tag);

            return tag;
        }

        private VectorTag CreateVectorTag(ITagModel tagModel)
        {
            VectorTag tag = new VectorTag();
            SetTagBasePropertyBinding(tag);
            SetLineTextTagBasePropertyBindings(tag);
            SetVectorTagPropertyBindings(tag);

            return tag;
        }

        private void SetTagBasePropertyBinding(TagBase tag)
        {
            Binding bindName = new Binding("TagName");
            tag.SetBinding(TagBase.TagNameProperty, bindName);

            Binding bindTagNameVisibility = new Binding("TagNameVisibility");
            tag.SetBinding(TagBase.TagNameVisibilityProperty, bindTagNameVisibility);

            Binding bindForeground = new Binding("Foreground");
            tag.SetBinding(TagBase.ForegroundProperty, bindForeground);

            Binding bindBackground = new Binding("Background");
            tag.SetBinding(TagBase.BackgroundProperty, bindBackground);

            Binding bindFontSize = new Binding("FontSize");
            tag.SetBinding(TagBase.FontSizeProperty, bindFontSize);

            Binding bindLocation = new Binding("Location");
            tag.SetBinding(TagBase.LocationProperty, bindLocation);

            Binding bindIsCheckable = new Binding("IsCheckable");
            tag.SetBinding(TagBase.IsCheckableProperty, bindIsCheckable);

            Binding bindIsChecked = new Binding("IsChecked");
            bindIsChecked.Mode = BindingMode.TwoWay;
            bindIsChecked.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            tag.SetBinding(TagBase.IsCheckedProperty, bindIsChecked);

            Binding bindIsEnabled = new Binding("IsEnabled");
            tag.SetBinding(TagBase.IsTagEnabledProperty, bindIsEnabled);
        }

        private void SetIconTagPropertyBindings(TagBase tag)
        {
            Binding bindIcon = new Binding("ExtraData.Icon");
            tag.SetBinding(IconTag.IconProperty, bindIcon);

            Binding bindMouseOverIcon = new Binding("ExtraData.Icon");
            tag.SetBinding(IconTag.MouseOverIconProperty, bindMouseOverIcon);

            Binding bindMouseDownIcon = new Binding("ExtraData.Icon");
            tag.SetBinding(IconTag.MouseDownIconProperty, bindMouseDownIcon);
        }

        private void SetTextTagPropertyBindings(TagBase tag)
        {

        }

        private void SetVectorTagPropertyBindings(TagBase tag)
        {
            Binding bindAreaPoints = new Binding("ExtraData.AreaPoints");
            tag.SetBinding(VectorTag.AreaPointsProperty, bindAreaPoints);

            Binding bindArrowBrush = new Binding("ExtraData.ArrowBrush");
            tag.SetBinding(VectorTag.ArrowBrushProperty, bindArrowBrush);
        }

        private void SetAreaTagPropertyBindings(TagBase tag)
        {
            Binding bindAreaPoints = new Binding("ExtraData.AreaPoints");
            tag.SetBinding(AreaTag.AreaPointsProperty, bindAreaPoints);

            Binding bindAreaColor = new Binding("ExtraData.AreaColor");
            tag.SetBinding(AreaTag.AreaColorProperty, bindAreaColor);
        }

        private void SetLineTextTagBasePropertyBindings(TagBase tag)
        {
            Binding bindXOffset = new Binding("ExtraData.TextTagXOffset");
            tag.SetBinding(LineTextTagBase.TextTagXOffsetProperty, bindXOffset);

            Binding bindYOffset = new Binding("ExtraData.TextTagYOffset");
            tag.SetBinding(LineTextTagBase.TextTagYOffsetProperty, bindYOffset);

            Binding bindBackground = new Binding("ExtraData.TextTagBackground");
            tag.SetBinding(LineTextTagBase.TextTagBackgroundProperty, bindBackground);

            Binding bindBorderBrush = new Binding("ExtraData.TextTagBorderBrush");
            tag.SetBinding(LineTextTagBase.TextTagBorderBrushProperty, bindBorderBrush);
        }

        #endregion

        #region Protected Methods

        #endregion

        #region Public Methods

        public TagBase Create(ITagModel tagModel)
        {
            TagBase tag = null;
            switch (tagModel.Type)
            {
                case TagType.Text:
                    tag = CreateTextTag(tagModel);
                    break;
                case TagType.Icon:
                    tag = CreateIconTag(tagModel);
                    break;
                case TagType.Area:
                    tag = CreateAreaTag(tagModel);
                    break;
                case TagType.Vector:
                    tag = CreateVectorTag(tagModel);
                    break;
            }

            if (tag != null)
            {
                tag.Id = tagModel.Id;
                tag.DataContext = tagModel;
            }

            return tag;
        }

        #endregion
    }
}
