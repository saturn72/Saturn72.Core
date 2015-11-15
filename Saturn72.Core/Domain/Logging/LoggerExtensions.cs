using System;
using System.Collections.Generic;
using Saturn72.Core.Domain.Users;

namespace Saturn72.Core.Domain.Logging
{
    public static class LoggerExtensions
    {
        public static void Log(this IEnumerable<ILogger> loggers, Action<ILogger> action)
        {
            foreach (var l in loggers)
                action(l);
        }
        public static void Debug(this ILogger logger, string message, Exception exception = null,
            User user=null)
        {
            FilteredLog(logger, LogLevel.Debug, message, exception, user);
        }

        public static void Information(this ILogger logger, string message, Exception exception = null,
            User user=null)
        {
            FilteredLog(logger, LogLevel.Information, message, exception, user);
        }

        public static void Warning(this ILogger logger, string message, Exception exception = null,
            User user=null)
        {
            FilteredLog(logger, LogLevel.Warning, message, exception, user);
        }

        public static void Error(this ILogger logger, string message, Exception exception = null,
            User user=null)
        {
            FilteredLog(logger, LogLevel.Error, message, exception, user);
        }

        public static void Fatal(this ILogger logger, string message, Exception exception = null,
            User user=null)
        {
            FilteredLog(logger, LogLevel.Fatal, message, exception, user);
        }

        private static void FilteredLog(ILogger logger, LogLevel level, string message, Exception exception = null,
            User user=null)
        {
            //don't log thread abort exception
            if (exception is System.Threading.ThreadAbortException)
                return;

            if (logger.IsEnabled(level))
            {
                string fullMessage = exception == null ? string.Empty : exception.ToString();
                logger.InsertLog(level, message, fullMessage, user);
            }
        }
    }
}