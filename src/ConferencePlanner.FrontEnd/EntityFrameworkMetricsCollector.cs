using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using ConferencePlanner.Common.Metrics;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Internal;

namespace ConferencePlanner.FrontEnd
{
    public class EntityFrameworkMetricsCollector : IMetricsCollector
    {
        private readonly IDiagnosticsLogger<DbLoggerCategory.Database.Command> _diagnosticsLogger;

        public EntityFrameworkMetricsCollector(IDiagnosticsLogger<DbLoggerCategory.Database.Command> diagnosticsLogger)
        {
            _diagnosticsLogger = diagnosticsLogger;
        }

        public IDisposable Subscribe(IMetricsService metricsService)
        {
            // PROBABLY AWFUL
            if(_diagnosticsLogger.DiagnosticSource is DiagnosticListener listener)
            {
                return listener.Subscribe(new EventObserver());
            }
            return NullDisposable.Instance;
        }

        private class NullDisposable : IDisposable
        {
            public static readonly NullDisposable Instance = new NullDisposable();
            private NullDisposable() { }
            public void Dispose() { }
        }

        private class EventObserver : IObserver<KeyValuePair<string, object>>
        {
            public void OnCompleted()
            {
            }

            public void OnError(Exception error)
            {
            }

            public void OnNext(KeyValuePair<string, object> value)
            {
            }
        }
    }
}
