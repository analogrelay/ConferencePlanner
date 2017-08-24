using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Nest;

namespace ConferencePlanner.Common.Metrics
{
    public class ElasticSearchMetricsSink : IMetricsSink
    {
        private static readonly string DefaultIndexPrefix = "metrics-";
        private readonly ElasticClient _client;
        private readonly string _indexPrefix;
        private readonly ILogger<ElasticSearchMetricsSink> _logger;

        public ElasticSearchMetricsSink(IOptions<ElasticSearchMetricsOptions> options, ILogger<ElasticSearchMetricsSink> logger)
        {
            _indexPrefix = string.IsNullOrEmpty(options.Value.IndexPrefix) ? DefaultIndexPrefix : options.Value.IndexPrefix;
            var config = new ConnectionSettings(new Uri(options.Value.Address));
            if(!string.IsNullOrEmpty(options.Value.Username))
            {
                config.BasicAuthentication(options.Value.Username, options.Value.Password);
            }
            _client = new ElasticClient(config);
            _logger = logger;
        }

        public void Write(string measurement, double value, IDictionary<string, object> fields, IDictionary<string, string> tags, DateTime? timestamp)
        {
            // TODO: Batching
            // TODO: Cache index name
            var indexName = _indexPrefix + DateTime.UtcNow.ToString("yyyy.MM.dd");
            var request = new IndexRequest<Metric>(indexName, "doc")
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

            _logger.LogTrace("Writing metric to Elasticsearch");
            var resp = _client.Index(request);
            if(resp.IsValid)
            {
                _logger.LogTrace("Wrote metric");
            }
            else
            {
                _logger.LogError("Failed to write metric. Error: {Error}", resp.ServerError.ToString());
            }
        }

        private class Metric
        {
            [Date(Name="@timestamp")]
            public DateTime TimestampUtc { get; set; }
            public string Measurement { get; set; }
            public double Value { get; set; }
            public IDictionary<string, object> Fields { get; set; }
            public IDictionary<string, string> Tags { get; set; }
        }
    }
}
