using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ConferencePlanner.Common.Metrics;

namespace ConferencePlanner.FrontEnd
{
    public class HttpClientMetricRecorder
    {
        private readonly IMetricsService _metricsService;

        public HttpClientMetricRecorder(IMetricsService metricsService)
        {
            _metricsService = metricsService;
        }
    }
}
