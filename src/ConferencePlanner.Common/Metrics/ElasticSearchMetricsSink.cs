using System;
using System.Collections.Generic;
using Microsoft.Extensions.Options;
using Nest;

namespace ConferencePlanner.Common.Metrics
{
    public class ElasticSearchMetricsSink : IMetricsSink
    {
        private static readonly string DefaultIndexPrefix = "metrics-";
        private readonly ElasticClient _client;
        private readonly string _indexPrefix;

        public ElasticSearchMetricsSink(IOptions<ElasticSearchMetricsOptions> options)
        {
            _indexPrefix = string.IsNullOrEmpty(options.Value.IndexPrefix) ? DefaultIndexPrefix : options.Value.IndexPrefix;
            var config = new ConnectionSettings(new Uri(options.Value.Address));
            _client = new ElasticClient(config);
        }

        public void Write(string measurement, double value, IDictionary<string, object> fields, IDictionary<string, string> tags, DateTime? timestamp)
        {
            // TODO: Batching
            // TODO: Cache index name
            var indexName = _indexPrefix + DateTime.UtcNow.ToString("yyyy.MM.dd");
            var request = new IndexRequest<Metric>(indexName, measurement)
            {
                Document = new Metric()
                {
                    Measurement = measurement,
                    Value = value,
                    Fields = fields,
                    Tags = tags,
                    TimestampUtc = DateTime.UtcNow,
                }
            };

            _client.Index(request);
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
