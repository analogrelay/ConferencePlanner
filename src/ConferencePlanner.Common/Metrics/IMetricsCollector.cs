using System;
using System.Collections.Generic;
using System.Text;

namespace ConferencePlanner.Common.Metrics
{
    public interface IMetricsCollector
    {
        IDisposable Subscribe(IMetricsService metricsService);
    }
}
