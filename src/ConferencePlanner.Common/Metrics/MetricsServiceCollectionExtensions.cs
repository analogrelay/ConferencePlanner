using System;
using ConferencePlanner.Common.Metrics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class MetricsServiceCollectionExtensions
    {
        public static void AddMetrics(this IServiceCollection self)
        {
            self.TryAdd(ServiceDescriptor.Singleton(typeof(IMetricsService), typeof(MetricsService)));
            self.TryAdd(ServiceDescriptor.Singleton(typeof(IHostedService), typeof(MetricsCollectionService)));
        }

        public static void AddInfluxMetrics(this IServiceCollection self)
        {
            self.AddMetrics();
            self.TryAdd(ServiceDescriptor.Singleton(typeof(IMetricsSink), typeof(InfluxMetricsSink)));
        }

        public static void AddInfluxMetrics(this IServiceCollection self, Action<InfluxMetricsOptions> configure)
        {
            self.AddInfluxMetrics();
            self.Configure(configure);
        }

        public static void AddInfluxMetrics(this IServiceCollection self, IConfiguration configurationSection)
        {
            self.AddInfluxMetrics();
            self.Configure<InfluxMetricsOptions>(configurationSection);
        }
    }
}