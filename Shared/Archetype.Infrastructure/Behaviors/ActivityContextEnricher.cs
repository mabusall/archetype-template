namespace Archetype.Infrastructure.Behaviors;

internal class ActivityContextEnricher : ILogEventEnricher
{
    public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
    {
        Activity? activity = Activity.Current;
        if (activity is null)
            return;

        logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty("TraceId", activity.TraceId.ToString()));
        logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty("SpanId", activity.SpanId.ToString()));

        if (!string.IsNullOrEmpty(activity.ParentId))
            logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty("ParentId", activity.ParentId));
    }
}