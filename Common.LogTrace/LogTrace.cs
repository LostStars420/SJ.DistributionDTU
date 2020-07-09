using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace Common.LogTrace
{
    public class CLog
    {
        public static void LogCritical(string strMessage)
        {
            _ts.TraceEvent(TraceEventType.Critical,
                           0,
                           "   [{0}{1}]: {2}",
                           DateTime.Now.ToLongDateString(),
                           DateTime.Now.ToLongTimeString(),
                           strMessage);
        }

        public static void LogError(string strMessage)
        {
            _ts.TraceEvent(TraceEventType.Error,
                           0,
                           "      [{0}{1}]: {2}",
                           DateTime.Now.ToLongDateString(),
                           DateTime.Now.ToLongTimeString(),
                           strMessage);
            Debug.WriteLine(strMessage);
        }
        public static void LogError(Exception ex)
        {
            var strBuilder = new StringBuilder(ex.Message.Length + ex.StackTrace.Length + 10);
            strBuilder.AppendLine(ex.Message);
            strBuilder.AppendLine(ex.StackTrace);
            var str = strBuilder.ToString();
            _ts.TraceEvent(TraceEventType.Error,
                           0,
                           "      [{0}{1}]: {2}",
                           DateTime.Now.ToLongDateString(),
                           DateTime.Now.ToLongTimeString(),
                           str);
            Debug.WriteLine(str);
        }
        public static void LogWarning(string strMessage)
        {
            _ts.TraceEvent(TraceEventType.Warning,
                           0,
                           "    [{0}{1}]: {2}",
                           DateTime.Now.ToLongDateString(),
                           DateTime.Now.ToLongTimeString(),
                           strMessage);
            Debug.WriteLine(strMessage);
        }

        public static void LogInformation(string strMessage)
        {
            _ts.TraceEvent(TraceEventType.Information,
                            0,
                           "[{0}{1}]: {2}",
                           DateTime.Now.ToLongDateString(),
                           DateTime.Now.ToLongTimeString(),
                           strMessage);
            Debug.WriteLine(strMessage);
        }

        private static TraceSource _ts = new TraceSource("LogTrace");
    }
}
