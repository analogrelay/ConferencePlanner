using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Options;

namespace ConferencePlanner.Common.Metrics
{
    public class MetricsService : IMetricsService
    {
        private readonly IEnumerable<IMetricsSink> _sinks;

        public MetricsService(IEnumerable<IMetricsSink> sinks)
        {
            _sinks = sinks;
        }

        public void Write(string measurement, IReadOnlyDictionary<string, object> fields, IReadOnlyDictionary<string, string> tags, DateTime? timestamp)
        {
            foreach(var sink in _sinks)
            {
                sink.Write(measurement, fields, tags, timestamp);
            }
        }
    }
}
