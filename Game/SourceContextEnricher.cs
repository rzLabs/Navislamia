using Microsoft.Extensions.Logging;
using Serilog.Core;
using Serilog.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Navislamia.Game.Logging.Enrichers;

public class SourceContextEnricher : ILogEventEnricher
{
    public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
    {
        if (logEvent.Properties.TryGetValue("SourceContext", out var property))
        {
            var propertyVal = ((ScalarValue)property).Value as string;

            if (propertyVal is not null)
            {
                var lastElement = propertyVal.Split('.').LastOrDefault();

                if (lastElement is not null)
                {
                    logEvent.AddOrUpdateProperty(new LogEventProperty("SourceContext", new ScalarValue(lastElement)));
                }
            }
        }
    }
}
