﻿using System;

namespace Saturn72.Core.Events
{
    public abstract class EventBase
    {
        protected EventBase()
        {
            FiredOn = DateTime.Now;
        }

        public object Id { get; set; }
        public object InvocationId { get; set; }
        public DateTime FiredOn { get; set; }
    }
}