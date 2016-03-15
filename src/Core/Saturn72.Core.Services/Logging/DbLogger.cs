using System;
using System.Collections.Generic;
using System.Linq;
using Saturn72.Core.Data;
using Saturn72.Core.Domain.Common;
using Saturn72.Core.Domain.Logging;
using Saturn72.Core.Logging;

namespace Saturn72.Core.Services.Logging
{
    public class DbLogger : ILogger
    {
        private readonly CommonSettings _commonSettings;
        private readonly IRepository<LogRecord> _logRepository;

        public DbLogger(IRepository<LogRecord> logRepository, CommonSettings commonSettings)
        {
            _logRepository = logRepository;
            _commonSettings = commonSettings;
        }

        public bool IsEnabled(LogLevel level)
        {
            return true;
        }

        public LogRecord InsertLogRecord(LogLevel logLevel, string shortMessage, string fullMessage = "", IDictionary<object, object> extraData = null)
        {
            //check ignore word/phrase list?
            if (IgnoreLog(shortMessage) || IgnoreLog(fullMessage))
                return null;

            var log = new LogRecord
            {
                LogLevel = logLevel,
                ShortMessage = shortMessage,
                FullMessage = fullMessage,
                CreatedOnUtc = DateTime.UtcNow
            };

            _logRepository.Insert(log);

            return log;
        }

        #region Utitilities

        /// <summary>
        ///     Gets a value indicating whether this message should not be logged
        /// </summary>
        /// <param name="message">TextMessage</param>
        /// <returns>ActualValue</returns>
        protected virtual bool IgnoreLog(string message)
        {
            if (_commonSettings.IgnoreLogWordlist.Count == 0)
                return false;

            if (string.IsNullOrWhiteSpace(message))
                return false;

            return _commonSettings
                .IgnoreLogWordlist
                .Any(x => message.IndexOf(x, StringComparison.InvariantCultureIgnoreCase) >= 0);
        }
        #endregion
    }
}