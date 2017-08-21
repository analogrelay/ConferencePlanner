using System;
using System.Collections.Generic;
using Microsoft.Extensions.Options;
using Nest;

namespace ConferencePlanner.Common.Metrics
{
    public class ElasticSearchMetricsSink : IMetricsSink
    {
        private static readonly string DefaultIndexName = "metrics";
        private readonly ElasticClient _client;

        public ElasticSearchMetricsSink(IOptions<ElasticSearchMetricsOptions> options)
        {
            var indexName = string.IsNullOrEmpty(options.Value.IndexName) ? DefaultIndexName : options.Value.IndexName;
            var config = new ConnectionSettings(new Uri(options.Value.Address))
                .InferMappingFor<Metric>(m => m.IndexName(indexName).TypeName("metric"));
            _client = new ElasticClient(config);
        }

        public void Write(string measurement, double value, IDictionary<string, object> fields, IDictionary<string, string> tags, DateTime? timestamp)
        {
            // TODO: Batching
            _client.Index(new Metric
            {
                Measurement = measurement,
                Value = value,
                Fields = fields,
                Tags = tags,
                TimestampUtc = DateTime.UtcNow,
            });
        }

        private class Metric
        {
            public string Measurement { get; set; }
            public double Value { get; set; }
            public IDictionary<string, object> Fields { get; set; }
            public IDictionary<string, string> Tags { get; set; }
            public DateTime TimestampUtc { get; set; }
        }
    }
}
