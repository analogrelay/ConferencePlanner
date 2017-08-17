using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.Extensibility;

namespace ConferencePlanner.BackEnd
{
    internal class ServiceNameTelemetryInitializer : ITelemetryInitializer
    {
        public string ServiceName { get; }

        public ServiceNameTelemetryInitializer(string serviceName)
        {
            ServiceName = serviceName;
        }

        public void Initialize(ITelemetry telemetry)
        {
            telemetry.Context.Cloud.RoleName = ServiceName;
        }
    }
}
