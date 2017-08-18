using System.Collections.Generic;

namespace ConferencePlanner.Common.Metrics
{
    public class MetricsOptions
    {
        public IList<IMetricsSink> Sinks { get; } = new List<IMetricsSink>();
    }
}
