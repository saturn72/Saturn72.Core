using System.Collections.Generic;
using Automation.Core.Configuration;

namespace Automation.Core.Domain.Common
{
    public class CommonSettings : ISettings
    {
        public CommonSettings()
        {
            IgnoreLogWordlist = new List<string>();
        }

        /// <summary>
        /// Gets or sets a value indicating whether full-text search is supported
        /// </summary>
        public bool UseFullTextSearch { get; set; }

        /// <summary>
        /// Gets or sets a Full-Text search mode
        /// </summary>
        public FulltextSearchMode FullTextMode { get; set; }

        /// <summary>
        /// Gets or sets a ignore words (phrases) to be ignored when logging errors/messages
        /// </summary>
        public List<string> IgnoreLogWordlist { get; set; }
    }
}