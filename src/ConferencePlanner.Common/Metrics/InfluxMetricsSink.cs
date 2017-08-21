using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using InfluxDB.Collector;
using InfluxDB.Collector.Pipeline;
using InfluxDB.LineProtocol.Client;
using InfluxDB.LineProtocol.Payload;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ConferencePlanner.Common.Metrics
{
    public class InfluxMetricsSink : IMetricsSink
    {
        private readonly MetricsCollector _collector;
        private readonly ILogger<InfluxMetricsSink> _logger;
        private readonly LineProtocolClient _client;

        public InfluxMetricsSink(IHostingEnvironment hostingEnvironment, IOptions<InfluxMetricsOptions> options, ILogger<InfluxMetricsSink> logger)
        {
            _client = new LineProtocolClient(new Uri(options.Value.Address), options.Value.Database, options.Value.Username, options.Value.Password);

            var config = new CollectorConfiguration();
            if(options.Value.ApplyDefaultTags)
            {
                config.Tag.With("ApplicationName", hostingEnvironment.ApplicationName);
                config.Tag.With("EnvironmentName", hostingEnvironment.EnvironmentName);
            }
            config.Batch.AtInterval(TimeSpan.FromSeconds(options.Value.BatchIntervalSeconds));
            config.WriteTo.Emitter(WritePoints);

            options.Value.AdditionalConfiguration?.Invoke(config);

            _collector = config.CreateCollector();
            _logger = logger;
        }

        public void Write(string measurement, double value, IDictionary<string, object> fields, IDictionary<string, string> tags = null, DateTime? timestamp = null)
        {
            fields["value"] = value;
            _collector.Measure(measurement, fields, new ReadOnlyDictionary<string, string>(tags));
        }

        private void WritePoints(PointData[] points)
        {
            var payload = new LineProtocolPayload();
            foreach(var point in points)
            {
                payload.Add(new LineProtocolPoint(point.Measurement, point.Fields, point.Tags, point.UtcTimestamp));
            }

            _logger.LogDebug("Writing batch of {batchCount} points to InfluxDb", points.Length);
            var result = _client.WriteAsync(payload).Result;
            if(result.Success)
            {
                _logger.LogDebug("Batch written successfully");
            }
            else
            {
                _logger.LogError("Failed to write batch to InfluxDb. Error: {errorMessage}", result.ErrorMessage);
            }
        }
    }
}
