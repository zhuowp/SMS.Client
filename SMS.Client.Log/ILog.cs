using System;

namespace SMS.Client.Log
{
    public interface ILog
    {
        void Debug(object message);
        void DebugFormatted(string format, params object[] args);
        void Info(object message);
        void InfoFormatted(string format, params object[] args);
        void Warn(object message);
        void Warn(object message, Exception exception);
        void WarnFormatted(string format, params object[] args);
        void Error(object message);
        void Error(object message, Exception exception);
        void ErrorFormatted(string format, params object[] args);
        void Fatal(object message);
        void Fatal(object message, Exception exception);
        void FatalFormatted(string format, params object[] args);
    }
}
