using SMS.Client.Common.Utilities;
using SMS.Client.Host.Models;
using SMS.Client.Log;
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

        private readonly string _themeFilePath = string.Empty;
        private ThemeModel _currentTheme = null;

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
        {
            _themeFilePath = Path.Combine(Environment.CurrentDirectory, @"Configs\Theme.json");
        }

        #endregion

        #region Private Methods

        private void UpdateThemeResource(ThemeModel theme)
        {
            _currentTheme = theme;

            string uri = string.Format("pack://application:,,,/{0};component/{1}", theme.AssemblyName, theme.EntrancePath);
            ResourceDictionary resource = new ResourceDictionary()
            {
                Source = new Uri(uri, UriKind.Absolute)
            };

            //将资源字典合并到当前资源中
            Application.Current.Resources.MergedDictionaries.Clear();
            Application.Current.Resources.MergedDictionaries.Add(resource);
        }

        #endregion

        #region Protected Methods

        #endregion

        #region Public Methods

        public void LoadThemeResources()
        {
            try
            {
                ThemeModel theme = _themeFilePath.FromJsonFile<ThemeModel>();
                if (theme == null)
                {
                    theme = new ThemeModel()
                    {
                        Id = "536db577-f136-4c67-ac71-0105f212f02a",
                        ThemeName = "默认主题",
                        AssemblyName = "SMS.Client.Theme",
                        EntrancePath = "Themes/Generic.xaml",
                        ThemeIconPath = ""
                    };
                }

                UpdateThemeResource(theme);
            }
            catch (Exception ex)
            {
                LogHelper.Default.Error("加载主题资源错误：", ex);
            }
        }

        public void SetTheme(ThemeModel theme)
        {
            if (theme == null)
            {
                LogHelper.Default.Debug("主题设置失败，数据为空");
                return;
            }

            if (_currentTheme != null || _currentTheme.Id == theme.Id)
            {
                LogHelper.Default.Debug("所设置的主题与当前进程的主题相同，无需重新设置。");
                return;
            }

            theme.ToJsonFile(_themeFilePath);
            UpdateThemeResource(theme);
        }

        #endregion
    }
}
