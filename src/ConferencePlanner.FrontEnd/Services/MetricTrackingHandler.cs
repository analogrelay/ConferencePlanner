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
        private static readonly string MeasurementName = "HttpRequest";

        private readonly IMetricsService _metrics;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public MetricTrackingHandler(IHttpContextAccessor httpContextAccessor, IMetricsService metricsService, HttpMessageHandler innerHandler) : base(innerHandler)
        {
            _httpContextAccessor = httpContextAccessor;
            _metrics = metricsService;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var fields = new Dictionary<string, object>();
            var tags = new Dictionary<string, string>();

            tags[nameof(request.RequestUri)] = request.RequestUri.ToString();
            tags[nameof(request.Method)] = request.Method.ToString();
            tags[nameof(_httpContextAccessor.HttpContext.TraceIdentifier)] = _httpContextAccessor.HttpContext.TraceIdentifier;
            tags["RequestId"] = Activity.Current?.Id ?? _httpContextAccessor.HttpContext.TraceIdentifier;

            var stopwatch = Stopwatch.StartNew();
            HttpResponseMessage response = null;
            try
            {
                response = await base.SendAsync(request, cancellationToken);
                stopwatch.Stop();
                tags["Success"] = "true";
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                tags["Success"] = "false";
                tags["Exception"] = ex.ToString().Replace("\n", " ").Replace("\r", "");
                tags[nameof(response.StatusCode)] = "ConnectionFailure";

                // Rethrow, but finally block should trace exception info
                throw;
            }
            finally
            {
                // If we got here, the request completed, but still may have been a failed status code.
                if (response != null)
                {
                    tags[nameof(response.StatusCode)] = ((int)response.StatusCode).ToString();
                    tags[nameof(response.ReasonPhrase)] = response.ReasonPhrase.Replace("\n", " ").Replace("\r", "");
                }

                _metrics.Write(MeasurementName, stopwatch.Elapsed.TotalMilliseconds, fields, tags, timestamp: null);

            }
            return response;
        }
    }
}
