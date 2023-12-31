using System.Linq;
using Serilog.Core;
using Serilog.Events;

namespace Navislamia.Game;

public class SourceContextEnricher : ILogEventEnricher
{
    public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
    {
        if (!logEvent.Properties.TryGetValue("SourceContext", out var property))
        {
            return;
        }
        
        var propertyVal = ((ScalarValue)property).Value as string;
        var lastElement = propertyVal?.Split('.').LastOrDefault();

        if (!string.IsNullOrWhiteSpace(lastElement))
        {
            logEvent.AddOrUpdateProperty(new LogEventProperty("SourceContext", new ScalarValue(lastElement)));
        }
    }
}
