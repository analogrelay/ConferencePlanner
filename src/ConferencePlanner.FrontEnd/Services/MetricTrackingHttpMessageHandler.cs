using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using App.Metrics;
using ConferencePlanner.Common.Metrics;

namespace ConferencePlanner.FrontEnd.Services
{
    public class MetricTrackingHttpMessageHandler : DelegatingHandler
    {
        private IMetrics _metrics;

        public MetricTrackingHttpMessageHandler(IMetrics metrics, HttpMessageHandler innerHandler) : base(innerHandler)
        {
            _metrics = metrics;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            HttpResponseMessage resp;
            using (_metrics.Measure.Timer.Time(MetricsRegistry.BackendHttpRequestDuration))
            {
                resp = await base.SendAsync(request, cancellationToken);
            }
            _metrics.Measure.Meter.Mark(MetricsRegistry.BackendHttpRequestCount, ((int)resp.StatusCode).ToString());
            return resp;
        }
    }
}
