using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ConferencePlanner.Common.Metrics
{
    public static class MetricsHelper
    {
        public static void RegisterMetrics(IServiceCollection services, IConfiguration configuration)
        {
            services.AddMetrics();
            var influxConfig = configuration.GetSection("Metrics:InfluxDb");
            if (influxConfig.Exists())
            {
                services.AddInfluxMetrics(influxConfig);
            }

            var elasticSearchConfig = configuration.GetSection("Metrics:Elasticsearch");
            if(elasticSearchConfig.Exists())
            {
                services.AddElasticSearchMetrics(elasticSearchConfig);
            }
        }
    }
}
