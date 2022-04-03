using System;
using System.Collections.Generic;
using System.Text;

using Configuration;
using Scripting;
using Notification;
using Objects;

namespace Maps
{
    public interface IMapService
    {
        public bool Initialize(string directory, IConfigurationService configurationService, IScriptingService scriptService, INotificationService notificationService);

        public KSize MapCount { get; set; }
    }
}
