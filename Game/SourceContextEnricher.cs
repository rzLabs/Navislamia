using System.Linq;
using Serilog.Core;
using Serilog.Events;

namespace Navislamia.Game;

public class SourceContextEnricher : ILogEventEnricher
{
    public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
    {
        if (logEvent.Properties.TryGetValue("SourceContext", out var property))
        {
            var propertyVal = ((ScalarValue)property).Value as string;

            var lastElement = propertyVal?.Split('.').LastOrDefault();

            if (lastElement != null)
            {
                logEvent.AddOrUpdateProperty(new LogEventProperty("SourceContext", new ScalarValue(lastElement)));
            }
        }
    }
}
