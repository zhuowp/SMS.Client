using System;
using System.Collections.Generic;
using System.Text;

namespace SMS.Client.Log
{
    public class LogHelper
    {
        #region Fields

        private ILog _log = null;

        private static LogHelper _default = null;
        private static object _lock = new object();

        #endregion

        #region Properties

        public static LogHelper Default
        {
            get
            {
                if (_default == null)
                {
                    lock (_lock)
                    {
                        if (_default == null)
                        {
                            _default = new LogHelper("loginfo");
                        }
                    }
                }

                return _default;
            }
        }

        #endregion

        #region Constructors

        private LogHelper(string loggerName)
        {
            _log = new Log4NetAdapter(loggerName);
        }

        #endregion

        #region Private Methods

        #endregion

        #region Public Methods

        public static LogHelper GetInstance(string loggerName)
        {
            return new LogHelper(loggerName);
        }

        public void Debug(object message)
        {
            _log.Debug(message);
        }

        public void DebugFormatted(string format, params object[] args)
        {
            _log.DebugFormatted(format, args);
        }

        public void Error(object message)
        {
            _log.Error(message);
        }

        public void Error(object message, Exception exception)
        {
            _log.Error(message, exception);
        }

        public void ErrorFormatted(string format, params object[] args)
        {
            _log.ErrorFormatted(format, args);
        }

        public void Fatal(object message)
        {
            _log.Fatal(message);
        }

        public void Fatal(object message, Exception exception)
        {
            _log.Fatal(message, exception);
        }

        public void FatalFormatted(string format, params object[] args)
        {
            _log.FatalFormatted(format, args);
        }

        public void Info(object message)
        {
            _log.Info(message);
        }

        public void InfoFormatted(string format, params object[] args)
        {
            _log.InfoFormatted(format, args);
        }

        public void Warn(object message)
        {
            _log.Warn(message);
        }

        public void Warn(object message, Exception exception)
        {
            _log.Warn(message, exception);
        }

        public void WarnFormatted(string format, params object[] args)
        {
            _log.WarnFormatted(format, args);
        }

        #endregion

    }
}
