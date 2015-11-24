using System.Collections.Generic;
using Saturn72.Core.Domain.Logging;

namespace Saturn72.Core.Logging
{
    public interface ILogger
    {
        /// <summary>
        ///     Determines whether a log level is enabled
        /// </summary>
        /// <param name="level">Log level</param>
        /// <returns>ActualValue</returns>
        bool IsEnabled(LogLevel level);


        /// <summary>
        ///     Inserts a log item
        /// </summary>
        /// <param name="logLevel">Log level</param>
        /// <param name="shortMessage">The short message</param>
        /// <param name="fullMessage">The full message</param>
        /// <param name="extraData"></param>
        /// <returns>A log item</returns>
        LogRecord InsertLogRecord(LogLevel logLevel, string shortMessage, string fullMessage = "", IDictionary<object, object> extraData = null);
    }
}