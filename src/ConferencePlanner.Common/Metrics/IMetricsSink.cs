using System;
using System.Collections.Generic;

namespace ConferencePlanner.Common.Metrics
{
    public interface IMetricsSink
    {
        void Write(string measurement, double value, IDictionary<string, object> fields, IDictionary<string, string> tags, DateTime? timestamp);
    }
}
