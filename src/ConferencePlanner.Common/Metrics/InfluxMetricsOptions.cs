using System;
using System.Collections.Generic;
using InfluxDB.Collector;

namespace ConferencePlanner.Common.Metrics
{
    public class InfluxMetricsOptions
    {
        public static readonly int DefaultBatchIntervalInSeconds = 5;

        public string Address { get; set;  }
        public string Database { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public int BatchIntervalSeconds { get; set; } = DefaultBatchIntervalInSeconds;
        public bool ApplyDefaultTags { get; set; } = true;
        public IDictionary<string, string> Tags { get; set; } = new Dictionary<string, string>();
        public Action<CollectorConfiguration> AdditionalConfiguration { get; set; }
    }
}
