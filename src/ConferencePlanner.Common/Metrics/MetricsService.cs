using System;
using System.Collections.Generic;

namespace ConferencePlanner.Common.Metrics
{
    public class MetricsService : IMetricsService
    {
        private readonly IEnumerable<IMetricsSink> _sinks;

        public MetricsService(IEnumerable<IMetricsSink> sinks)
        {
            _sinks = sinks;
        }

        public void Write(string measurement, double value, IDictionary<string, object> fields, IDictionary<string, string> tags, DateTime? timestamp)
        {
            foreach(var sink in _sinks)
            {
                sink.Write(measurement, value, fields, tags, timestamp);
            }
        }
    }
}
