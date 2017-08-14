using System.Diagnostics.Tracing;

namespace BackEnd
{
    public class RequestCounterEventSource : EventSource
    {
        // define the singleton instance of the event source
        public static RequestCounterEventSource Log = new RequestCounterEventSource();
        private EventCounter _requestCounter;

        private RequestCounterEventSource()
        {
            _requestCounter = new EventCounter("request", this);
        }

        /// <summary>
        /// Call this method to indicate that a request for a URL was made which tool a particular amount of time
        public void Request(string url, float elapsedMSec)
        {
            // Notes:
            //   1. the event ID passed to WriteEvent (1) corresponds to the (implied) event ID
            //      assigned to this method. The event ID could have been explicitly declared
            //      using an EventAttribute for this method
            //   2. Each counter supports a single float value, so conceptually it maps to a single
            //      measurement in the code.
            //   3. You don't have to have 
            WriteEvent(1, url, elapsedMSec);    // This logs it to the event stream if events are on.    
            _requestCounter.WriteMetric(elapsedMSec);        // This adds it to the PerfCounter called 'Request' if PerfCounters are on
        }
    }
}
