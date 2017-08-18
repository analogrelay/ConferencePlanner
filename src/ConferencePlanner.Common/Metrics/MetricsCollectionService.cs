using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;

namespace ConferencePlanner.Common.Metrics
{
    public class MetricsCollectionService : IHostedService
    {
        private readonly IEnumerable<IMetricsCollector> _collectors;
        private readonly IMetricsService _metricsService;
        private List<IDisposable> _subscriptions;

        public MetricsCollectionService(IEnumerable<IMetricsCollector> collectors, IMetricsService metricsService)
        {
            _collectors = collectors;
            _metricsService = metricsService;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _subscriptions = new List<IDisposable>();
            foreach(var collector in _collectors)
            {
                _subscriptions.Add(collector.Subscribe(_metricsService));
            }
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            foreach(var subscription in _subscriptions)
            {
                subscription.Dispose();
            }
            return Task.CompletedTask;
        }
    }
}
