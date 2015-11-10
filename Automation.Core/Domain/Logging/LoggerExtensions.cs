using System;
using System.Collections.Generic;
using Automation.Core.Domain.ExecutionNodes;
using Automation.Core.Domain.Users;

namespace Automation.Core.Domain.Logging
{
    public static class LoggerExtensions
    {
        public static void Log(this IEnumerable<ILogger> loggers, Action<ILogger> action)
        {
            foreach (var l in loggers)
                action(l);
        }
        public static void Debug(this ILogger logger, string message, Exception exception = null,
            User user=null, ExecutionNode executionNode = null)
        {
            FilteredLog(logger, LogLevel.Debug, message, exception, user, executionNode);
        }

        public static void Information(this ILogger logger, string message, Exception exception = null,
            User user=null, ExecutionNode executionNode = null)
        {
            FilteredLog(logger, LogLevel.Information, message, exception, user, executionNode);
        }

        public static void Warning(this ILogger logger, string message, Exception exception = null,
            User user=null, ExecutionNode executionNode = null)
        {
            FilteredLog(logger, LogLevel.Warning, message, exception, user, executionNode);
        }

        public static void Error(this ILogger logger, string message, Exception exception = null,
            User user=null, ExecutionNode executionNode = null)
        {
            FilteredLog(logger, LogLevel.Error, message, exception, user, executionNode);
        }

        public static void Fatal(this ILogger logger, string message, Exception exception = null,
            User user=null, ExecutionNode executionNode = null)
        {
            FilteredLog(logger, LogLevel.Fatal, message, exception, user, executionNode);
        }

        private static void FilteredLog(ILogger logger, LogLevel level, string message, Exception exception = null,
            User user=null, ExecutionNode executionNode = null)
        {
            //don't log thread abort exception
            if (exception is System.Threading.ThreadAbortException)
                return;

            if (logger.IsEnabled(level))
            {
                string fullMessage = exception == null ? string.Empty : exception.ToString();
                logger.InsertLog(level, message, fullMessage, user, executionNode);
            }
        }
    }
}