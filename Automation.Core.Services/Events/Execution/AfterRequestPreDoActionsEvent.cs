﻿using Automation.Core.Activity;
using Automation.Core.Events;

namespace Automation.Core.Services.Events.Execution
{
    public class AfterRequestPreDoActionsEvent:EventBase
    {
        public RequestBase Request { get; set; }
    }
}
