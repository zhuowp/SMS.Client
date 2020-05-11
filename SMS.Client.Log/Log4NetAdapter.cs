using log4net;
using log4net.Config;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SMS.Client.Log
{
    public class Log4NetAdapter : ILog
    {
        #region Fields

        private log4net.ILog _logger = null;
        private static log4net.Repository.ILoggerRepository _repository = null;

        #endregion

        #region Constructors

        static Log4NetAdapter()
        {
            _repository = LogManager.CreateRepository("root");
        }

        public Log4NetAdapter(string loggerName)
        {
            string configPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Configs/log4net.config");
            FileInfo configFileInfo = new FileInfo(configPath);
            XmlConfigurator.ConfigureAndWatch(_repository, configFileInfo);

            _logger = LogManager.GetLogger(_repository.Name, loggerName);
        }

        #endregion

        #region Public Methods

        public void Debug(object message)
        {
            _logger.Debug(message);
        }

        public void DebugFormatted(string format, params object[] args)
        {
            _logger.DebugFormat(format, args);
        }

        public void Error(object message)
        {
            _logger.Error(message);
        }

        public void Error(object message, Exception exception)
        {
            _logger.Error(message, exception);
        }

        public void ErrorFormatted(string format, params object[] args)
        {
            _logger.ErrorFormat(format, args);
        }

        public void Fatal(object message)
        {
            _logger.Fatal(message);
        }

        public void Fatal(object message, Exception exception)
        {
            _logger.Fatal(message, exception);
        }

        public void FatalFormatted(string format, params object[] args)
        {
            _logger.FatalFormat(format, args);
        }

        public void Info(object message)
        {
            _logger.Info(message);
        }

        public void InfoFormatted(string format, params object[] args)
        {
            _logger.InfoFormat(format, args);
        }

        public void Warn(object message)
        {
            _logger.Warn(message);
        }

        public void Warn(object message, Exception exception)
        {
            _logger.Warn(message, exception);
        }

        public void WarnFormatted(string format, params object[] args)
        {
            _logger.WarnFormat(format, args);
        }

        #endregion
    }
}
