using Autofac;
using Configuration;
using Database;
using Maps;
using Navislamia.Command;
using Navislamia.Command.Commands;
using Navislamia.Data;
using Navislamia.Game;
using Network;
using Notification;
using Scripting;
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

            builder.RegisterType<CommandModule>().As<ICommandService>();
            builder.RegisterType<WaitCommand>();
            builder.RegisterType<GetCommand>();

            builder.RegisterType<DataModule>().As<IDataService>();
            builder.RegisterType<DatabaseModule>().As<IDatabaseService>();
            builder.RegisterType<ScriptModule>().As<IScriptingService>();
            builder.RegisterType<MapModule>().As<IMapService>();
            builder.RegisterType<NetworkModule>().As<INetworkService>();
            builder.RegisterType<GameModule>().As<IGameService>();

            return builder.Build();
        }
    }
}
