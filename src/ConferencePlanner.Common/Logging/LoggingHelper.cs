using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;
using Serilog.Extensions.Logging;
using Serilog.Sinks.Elasticsearch;

namespace ConferencePlanner.Common.Logging
{
    public static class LoggingHelper
    {
        public static void RegisterLogging(string service, ILoggingBuilder logging, IConfiguration configuration)
        {
            var loggerConfiguration = new LoggerConfiguration()
                .Enrich.WithEnvironmentUserName()
                .Enrich.WithMachineName()
                .Enrich.WithProcessId()
                .Enrich.WithThreadId()
                .Enrich.WithProperty("service", service)
                .Enrich.FromLogContext()
                .MinimumLevel.Is(LogEventLevel.Verbose);

            var elasticSearchSection = configuration.GetSection("Logging:ElasticSearch");
            if(elasticSearchSection.Exists())
            {
                var elasticSearchAddress = elasticSearchSection["Address"];
                if (!string.IsNullOrEmpty(elasticSearchAddress)) {
                    var sinkOptions = new ElasticsearchSinkOptions(new Uri(elasticSearchAddress));
                    loggerConfiguration.WriteTo.Elasticsearch(sinkOptions);
                }
            }

            logging.AddSerilog(loggerConfiguration.CreateLogger(), dispose: true);
        }
    }
}
