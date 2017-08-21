using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using ConferencePlanner.Common.Metrics;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Primitives;

namespace ConferencePlanner.BackEnd
{
    public class EntityFrameworkMetricsCollector : IMetricsCollector
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public EntityFrameworkMetricsCollector(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public IDisposable Subscribe(IMetricsService metricsService)
        {
            var listenerObserver = new ListenerObserver(metricsService, _httpContextAccessor);
            var subscription = DiagnosticListener.AllListeners.Subscribe(listenerObserver);
            return new PairDisposable(listenerObserver, subscription);
        }

        private class NullDisposable : IDisposable
        {
            public static readonly NullDisposable Instance = new NullDisposable();
            private NullDisposable() { }
            public void Dispose() { }
        }

        private class EventObserver : IObserver<KeyValuePair<string, object>>
        {
            private readonly IMetricsService _metricsService;
            private readonly IHttpContextAccessor _httpContextAccessor;
            private static Dictionary<string, Action<EventObserver, object>> _handlers = new Dictionary<string, Action<EventObserver, object>>()
            {
                [RelationalEventId.CommandExecuting.Name] = (o, p) => o.OnCommandExecuting((CommandEventData)p),
                [RelationalEventId.CommandExecuted.Name] = (o, p) => o.OnCommandExecuted((CommandEventData)p),
            };

            public EventObserver(IMetricsService metricsService, IHttpContextAccessor httpContextAccessor)
            {
                _metricsService = metricsService;
                _httpContextAccessor = httpContextAccessor;
            }

            public void OnCompleted()
            {
            }

            public void OnError(Exception error)
            {
            }

            public void OnNext(KeyValuePair<string, object> pair)
            {
                if(_handlers.TryGetValue(pair.Key, out var handler))
                {
                    handler(this, pair.Value);
                }
            }

            private void OnCommandExecuting(CommandEventData data)
            {
                data.Command.Site = new CommandTimer();
            }

            private void OnCommandExecuted(CommandEventData data)
            {
                if(data.Command.Site is CommandTimer timer)
                {
                    var time = timer.Stop();
                    var text = data.Command.CommandText
                        .Replace("\r", "")
                        .Replace("\n", " ");
                    _metricsService.Write(RelationalEventId.CommandExecuted.Name, new Dictionary<string, object>()
                    {
                        [nameof(data.Command.CommandText)] = text,
                        ["value"] = time.TotalMilliseconds,
                    },
                    new Dictionary<string, string>()
                    {
                        [nameof(data.Command.CommandText)] = text,
                        ["RequestId"] = (_httpContextAccessor.HttpContext?.Request?.Headers?["Request-Id"] ?? StringValues.Empty).ToString()
                    },
                    timestamp: null);
                }
            }

            // THIS IS SO WRONG BUT IT FEELS SO RIGHT
            private class CommandTimer : ISite
            {
                private readonly Stopwatch _stopwatch;

                public IComponent Component { get; set; }
                public IContainer Container { get; set; }
                public string Name { get; set; } = nameof(CommandTimer);
                public bool DesignMode => false;

                public CommandTimer()
                {
                    _stopwatch = Stopwatch.StartNew();
                }

                public object GetService(Type serviceType) => null;

                public TimeSpan Stop()
                {
                    _stopwatch.Stop();
                    return _stopwatch.Elapsed;
                }
            }
        }

        private class ListenerObserver : IDisposable, IObserver<DiagnosticListener>
        {
            private readonly IMetricsService _metricsService;
            private readonly IHttpContextAccessor _httpContextAccessor;
            private IDisposable _subscription;

            public ListenerObserver(IMetricsService metricsService, IHttpContextAccessor httpContextAccessor)
            {
                _metricsService = metricsService;
                _httpContextAccessor = httpContextAccessor;
            }

            public void Dispose()
            {
                _subscription.Dispose();
            }

            public void OnCompleted()
            {
            }

            public void OnError(Exception error)
            {
            }

            public void OnNext(DiagnosticListener value)
            {
                if (value.Name.Equals(DbLoggerCategory.Name))
                {
                    _subscription = value.Subscribe(new EventObserver(_metricsService, _httpContextAccessor));
                }
            }
        }

        private class PairDisposable : IDisposable
        {
            private IDisposable _first;
            private IDisposable _second;

            public PairDisposable(IDisposable first, IDisposable second)
            {
                _first = first;
                _second = second;
            }

            public void Dispose()
            {
                _first.Dispose();
                _second.Dispose();
            }
        }
    }
}
