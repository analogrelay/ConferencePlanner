using System;
using System.Collections.Generic;
using InfluxDB.Collector;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Options;

namespace ConferencePlanner.Common.Metrics
{
    public class InfluxMetricsSink : IMetricsSink
    {
        private readonly MetricsCollector _collector;

        public InfluxMetricsSink(IHostingEnvironment hostingEnvironment, IOptions<InfluxMetricsOptions> options)
        {
            var config = new CollectorConfiguration();
            if(options.Value.ApplyDefaultTags)
            {
                config.Tag.With("ApplicationName", hostingEnvironment.ApplicationName);
                config.Tag.With("EnvironmentName", hostingEnvironment.EnvironmentName);
            }
            config.Batch.AtInterval(TimeSpan.FromSeconds(options.Value.BatchIntervalSeconds));
            config.WriteTo.InfluxDB(options.Value.Address, options.Value.Database, options.Value.Username, options.Value.Password);

            options.Value.AdditionalConfiguration?.Invoke(config);

            _collector = config.CreateCollector();
        }

        public void Write(string measurement, IReadOnlyDictionary<string, object> fields, IReadOnlyDictionary<string, string> tags = null, DateTime? timestamp = null) => _collector.Write(measurement, fields, tags);
    }
}
