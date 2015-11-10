﻿using System;
using Automation.Core.Domain.ExecutionNodes;
using Automation.Core.Domain.Users;

namespace Automation.Core.Domain.Logging
{
    public class Log : BaseEntity
    {
        /// <summary>
        /// Gets or sets the log level identifier
        /// </summary>
        public int LogLevelId { get; set; }

        /// <summary>
        /// Gets or sets the short message
        /// </summary>
        public string ShortMessage { get; set; }

        /// <summary>
        /// Gets or sets the full exception
        /// </summary>
        public string FullMessage { get; set; }

        /// <summary>
        /// Gets or sets the execution node
        /// </summary>
        public int? ExecutionNodeId { get; set; }

        /// <summary>
        /// Gets or sets the date and time of instance creation
        /// </summary>
        public DateTime CreatedOnUtc { get; set; }

        /// <summary>
        /// Gets or sets the log level
        /// </summary>
        public LogLevel LogLevel
        {
            get
            {
                return (LogLevel)LogLevelId;
            }
            set
            {
                LogLevelId = (int)value;
            }
        }

        /// <summary>
        /// Gets or sets the execution Node
        /// </summary>
        public virtual ExecutionNode ExecutionNode { get; set; }

        public virtual User User { get; set; }
        public int? UserId { get; set; }
    }
}
