using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Linq;
using InfluxDB.Collector;

namespace BackEnd
{
    public class MetricCollectorEventListener : EventListener
    {
        private readonly MetricsCollector _collector;

        protected MetricCollectorEventListener(MetricsCollector collector)
        {
            _collector = collector;
        }

        protected override void OnEventSourceCreated(EventSource eventSource)
        {
            Console.WriteLine($"MCEL: EventSourceCreated: {eventSource.Name}");
            base.OnEventSourceCreated(eventSource);
        }

        protected override void OnEventWritten(EventWrittenEventArgs eventData)
        {
            var payloadDict = Enumerable.Zip(eventData.PayloadNames, eventData.Payload, (name, value) => new KeyValuePair<string, string>(name, value.ToString()))
                .ToDictionary(p => p.Key, p => p.Value);
            _collector.Increment("eventsource/" + eventData.EventSource.Name + "/" + eventData.EventName, tags: payloadDict);
        }
    }
}
