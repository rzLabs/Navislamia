using System;
using System.Collections.Generic;
using System.Text;

using Configuration;
using Scripting;
using Notification;

namespace Maps
{
    public interface IMapService
    {
        public bool Initialize(string directory, IConfigurationService configurationService, IScriptingService scriptService, INotificationService notificationService);
    }
}
