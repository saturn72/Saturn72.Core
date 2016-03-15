using System;
using System.Collections.Generic;
using System.Linq;
using Saturn72.Core.Data;
using Saturn72.Core.Domain.Logging;

namespace Saturn72.Core.Services.Logging
{
    public class DbLogRecordService : ILogRecordService
    {
        #region Ctor

        /// <summary>
        ///     Ctor
        /// </summary>
        /// <param name="logRepository">Log repository</param>
        public DbLogRecordService(IRepository<LogRecord> logRepository)
        {
            _logRepository = logRepository;
        }

        #endregion



        #region Fields

        private readonly IRepository<LogRecord> _logRepository;

        #endregion

        #region Methods

        /// <summary>
        ///     Deletes a log item
        /// </summary>
        /// <param name="log">Log item</param>
        public virtual void DeleteLog(LogRecord log)
        {
            if (log == null)
                throw new ArgumentNullException("log");

            _logRepository.Delete(log);
        }

        /// <summary>
        ///     Gets all log items
        /// </summary>
        /// <param name="fromUtc">Log item creation from; null to load all records</param>
        /// <param name="toUtc">Log item creation to; null to load all records</param>
        /// <param name="message">TextMessage</param>
        /// <param name="logLevel">Log level; null to load all records</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>Log item items</returns>
        public virtual IPagedList<LogRecord> GetAllLogs(DateTime? fromUtc = null, DateTime? toUtc = null,
            string message = "", LogLevel? logLevel = null,
            int pageIndex = 0, int pageSize = int.MaxValue)
        {
            var query = _logRepository.Table;
            if (fromUtc.HasValue)
                query = query.Where(l => fromUtc.Value <= l.CreatedOnUtc);
            if (toUtc.HasValue)
                query = query.Where(l => toUtc.Value >= l.CreatedOnUtc);
            if (logLevel.HasValue)
            {
                var logLevelId = (int) logLevel.Value;
                query = query.Where(l => logLevelId == l.LogLevelId);
            }
            if (!string.IsNullOrEmpty(message))
                query = query.Where(l => l.ShortMessage.Contains(message) || l.FullMessage.Contains(message));
            query = query.OrderByDescending(l => l.CreatedOnUtc);

            return new PagedList<LogRecord>(query, pageIndex, pageSize);
        }

        /// <summary>
        ///     Gets a log item
        /// </summary>
        /// <param name="logId">Log item identifier</param>
        /// <returns>Log item</returns>
        public virtual LogRecord GetLogById(object logId)
        {
            if (logId == null)
                return null;

            return _logRepository.GetById(logId);
        }

        /// <summary>
        ///     Get log items by identifiers
        /// </summary>
        /// <param name="logIds">Log item identifiers</param>
        /// <returns>Log items</returns>
        public virtual IList<LogRecord> GetLogByIds(int[] logIds)
        {
            if (logIds == null || logIds.Length == 0)
                return new List<LogRecord>();

            var query = from l in _logRepository.Table
                where logIds.Contains(l.Id)
                select l;
            var logItems = query.ToList();
            //sort by passed identifiers
            var sortedLogItems = new List<LogRecord>();
            foreach (var id in logIds)
            {
                var log = logItems.Find(x => x.Id == id);
                if (log != null)
                    sortedLogItems.Add(log);
            }
            return sortedLogItems;
        }

        #endregion
    }
}