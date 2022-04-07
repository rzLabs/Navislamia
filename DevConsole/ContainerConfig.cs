using Autofac;
using Configuration;
using Navislamia.Game;
using Notification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevConsole
{
    public static class ContainerConfig
    {
        public static IContainer Configure()
        {
            var builder = new ContainerBuilder();

            builder.RegisterType<Application>().As<IApplication>();
            builder.RegisterType<ConfigurationModule>().As<IConfigurationService>();
            builder.RegisterType<NotificationModule>().As<INotificationService>();
            builder.RegisterType<GameModule>().As<IGameService>();

            return builder.Build();
        }
    }
}
