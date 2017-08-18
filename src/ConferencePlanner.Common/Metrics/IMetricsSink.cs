using System;
using System.Collections.Generic;

namespace ConferencePlanner.Common.Metrics
{
    public interface IMetricsSink
    {
        void Write(string measurement, IReadOnlyDictionary<string, object> fields, IReadOnlyDictionary<string, string> tags, DateTime? timestamp);
    }
}
