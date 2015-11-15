using Automation.Core.Events;
using Automation.Core.Services.Events;

namespace Automation.Core.Services.Execution.Event
{
    public class InsertedToQueue<T>:EventBase
    {
        public T Item { get; set; }
    }
}
