using System;
using System.Collections.Generic;
using Automation.Core.Validation;
using Automation.Extensions;

namespace Automation.Core.Activity
{
    /// <summary>
    ///     Represents System Request reposnse object
    /// </summary>
    public sealed class ActivityResponse
    {
        private IDictionary<object, object> _parameters;
        private ICollection<ActivityResponse> _subResponses;

        public IDictionary<object, object> Parameters
        {
            get { return _parameters ?? (_parameters = new Dictionary<object, object>()); }
        }

        public ICollection<ActivityResponse> SubResponses
        {
            get { return _subResponses ?? (_subResponses = new List<ActivityResponse>()); }
        }

        public RequestBase Request { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public object ActualValue { get; set; }
        public TimeSpan ElapsedTime { get; set; }

        public ValidationPoint ValidationResult { get; set; }
        public ActivityResponse ParentResponse { get; set; }
    }
}