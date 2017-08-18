using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using ConferencePlanner.Common.Metrics;
using Microsoft.AspNetCore.Http;

namespace ConferencePlanner.FrontEnd.Services
{
    public class MetricTrackingHandler : DelegatingHandler
    {
        private readonly string _measurementName;
        private readonly IMetricsService _metrics;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public MetricTrackingHandler(IHttpContextAccessor httpContextAccessor, string measurementName, IMetricsService metricsService, HttpMessageHandler innerHandler) : base(innerHandler)
        {
            _httpContextAccessor = httpContextAccessor;
            _measurementName = measurementName;
            _metrics = metricsService;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var fields = new Dictionary<string, object>();
            var tags = new Dictionary<string, string>();

            fields[nameof(request.RequestUri)] = request.RequestUri.ToString();
            fields[nameof(request.Method)] = request.Method.ToString();
            fields[nameof(_httpContextAccessor.HttpContext.TraceIdentifier)] = _httpContextAccessor.HttpContext.TraceIdentifier;

            request.Headers.Add("FrontendRequestId", _httpContextAccessor.HttpContext.TraceIdentifier);

            var stopwatch = Stopwatch.StartNew();
            HttpResponseMessage response;
            try
            {
                response = await base.SendAsync(request, cancellationToken);
                fields["Success"] = true;
            }
            catch (Exception ex)
            {
                fields["Success"] = false;
                fields["Exception"] = ex.ToString();

                // Rethrow, but trace exception info
                _metrics.Write(_measurementName, fields, tags, timestamp: null);
                throw;
            }

            // If we got here, the request completed, but still may have been a failed status code.
            fields[nameof(response.StatusCode)] = response.StatusCode;
            fields[nameof(response.ReasonPhrase)] = response.ReasonPhrase;

            _metrics.Write(_measurementName, fields, tags, timestamp: null);

            return response;
        }
    }
}
