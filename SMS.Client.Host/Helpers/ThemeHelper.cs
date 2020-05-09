using SMS.Client.Common.Utilities;
using SMS.Client.Host.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows;

namespace SMS.Client.Host.Helpers
{
    public class ThemeHelper
    {
        #region Fields

        private static ThemeHelper _instance = null;
        private static object _lock = new object();

        #endregion

        #region Properties

        public static ThemeHelper Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        if (_instance == null)
                        {
                            _instance = new ThemeHelper();
                        }
                    }
                }

                return _instance;
            }
        }

        #endregion

        #region Constructors

        private ThemeHelper()
        { }

        #endregion

        #region Private Methods

        #endregion

        #region Protected Methods

        #endregion

        #region Public Methods

        public void LoadThemeResources()
        {
            try
            {
                string themeFilePath = Path.Combine(Environment.CurrentDirectory, @"Configs\Theme.json");
                ThemeModel theme = themeFilePath.FromJsonFile<ThemeModel>();
                if (theme == null)
                {
                    theme = new ThemeModel()
                    {
                        ThemeName = "默认主题",
                        AssemblyName = "SMS.Client.Theme",
                        EntrancePath = "Themes/Generic.xaml",
                        ThemeIconPath = ""
                    };
                }

                string uri = string.Format("pack://application:,,,/{0};component/{1}", theme.AssemblyName, theme.EntrancePath);
                ResourceDictionary resource = new ResourceDictionary()
                {
                    Source = new Uri(uri, UriKind.Absolute)
                };

                //将资源字典合并到当前资源中
                Application.Current.Resources.MergedDictionaries.Add(resource);
            }
            catch { }
        }

        #endregion
    }
}
