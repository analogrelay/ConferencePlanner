using App.Metrics;
using App.Metrics.Core.Options;
using App.Metrics.Tagging;

namespace ConferencePlanner.Common.Metrics
{
    public class MetricsRegistry
    {
        public static MeterOptions BackendHttpRequestCount = new MeterOptions()
        {
            Name = "Backend Http Request Count",
            Context = "Application.HttpRequests",
            MeasurementUnit = Unit.Requests,
        };

        public static TimerOptions BackendHttpRequestDuration = new TimerOptions()
        {
            Name = "Backend Http Request Duration",
            Context = "Application.HttpRequests",
            MeasurementUnit = Unit.Requests,
            DurationUnit = TimeUnit.Milliseconds,
            RateUnit = TimeUnit.Milliseconds,
        };
    }
}
