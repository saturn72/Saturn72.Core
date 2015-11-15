using System.Collections.Generic;
using Automation.Core.Domain.Reporting;
using Automation.Core.Services.Events.Invocation;

namespace Automation.Core.Services.Reporting
{
    public class Reporter : IReporter
    {
        private static int _id;
        private readonly IReportWriter _reportWriter;
        private Queue<ReportNode> _reportNodeQueue;

        public Reporter(IReportWriter reportWriter)
        {
            _reportWriter = reportWriter;
        }

        protected Queue<ReportNode> ReportNodeQueue
        {
            get { return _reportNodeQueue ?? (_reportNodeQueue = new Queue<ReportNode>()); }
        }

        public virtual void HandleEvent(InvocationStartEvent eventMessage)
        {
            AddReportNode(EventMessageToReportNode(eventMessage, "Invocation_Start"));
        }

        public virtual void HandleEvent(InvokingEvent eventMessage)
        {
            AddReportNode(EventMessageToReportNode(eventMessage, "Invocation_Code"));
        }

        public virtual void HandleEvent(InvocationEndEvent eventMessage)
        {
            AddReportNode(EventMessageToReportNode(eventMessage, "Invocation_End"));
        }

        private ReportNode EventMessageToReportNode(InvocationEventBase eventMessage, string actionName)
        {
            var reportNode = GetNewReportNode();
            reportNode.InvocationContextId = eventMessage.InvocationId;
            reportNode.ActionName = actionName;
            reportNode.Message = eventMessage.TextMessage;
            reportNode.ExecutionDateTime = eventMessage.FiredOn;
            reportNode.InvocationData = eventMessage.InvocationData;
            return reportNode;
        }

        private static ReportNode GetNewReportNode()
        {
            return new ReportNode {Id = SetReportNodeId()};
        }

        private static int SetReportNodeId()
        {
            return ++_id;
        }

        protected virtual void AddReportNode(ReportNode reportNode)
        {
            ReportNodeQueue.Enqueue(reportNode);
            _reportWriter.Write(ReportNodeQueue.Dequeue());
        }
    }
}