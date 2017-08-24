using System.Diagnostics;
using Serilog.Core;
using Serilog.Events;

namespace ConferencePlanner.Common.Logging
{
    public class ActivityIdEnricher : ILogEventEnricher
    {
        public ActivityIdEnricher()
        {
        }

        public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
        {
            if (!string.IsNullOrEmpty(Activity.Current.Id))
            {
                logEvent.AddOrUpdateProperty(propertyFactory.CreateProperty("ActivityId", Activity.Current.Id));
            }

            if (!string.IsNullOrEmpty(Activity.Current.ParentId))
            {
                logEvent.AddOrUpdateProperty(propertyFactory.CreateProperty("ParentActivityId", Activity.Current.ParentId));
            }
        }
    }
}
