using Automation.Core.Domain.Reporting;

namespace Automation.Core.Services.Reporting
{
    public interface IReportWriter
    {
        void Write(ReportNode reportNode);
    }

  
}