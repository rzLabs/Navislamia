using Configuration;
using Database;
using Maps;
using Microsoft.Extensions.DependencyInjection;
using Navislamia.Command;
using Navislamia.Command.Commands;
using Navislamia.Data;
using Navislamia.Game;
using Navislamia.World;

using Notification;
using Scripting;
using Spectre.Console.Cli;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Network;
using Navislamia.Network.Packets.Actions;
using Navislamia.Network.Packets.Actions.Interfaces;

namespace DevConsole
{
    public static class ContainerConfig
    {
        public static TypeRegistrar Configure()
        {
            var serviceCollection = new ServiceCollection();

            serviceCollection.AddSingleton<IApplication, Application>();
            serviceCollection.AddSingleton<IConfigurationService, ConfigurationModule>();
            serviceCollection.AddSingleton<INotificationService, NotificationModule>();

            serviceCollection.AddSingleton<ICommandService, CommandModule>();

            serviceCollection.AddSingleton<IDataService, DataModule>();
            serviceCollection.AddSingleton<IDatabaseService, DatabaseModule>();
            serviceCollection.AddSingleton<IWorldService, WorldModule>();
            serviceCollection.AddSingleton<IScriptingService, ScriptModule>();
            serviceCollection.AddSingleton<IMapService, MapModule>();
            serviceCollection.AddSingleton<INetworkService, NetworkModule>();
            serviceCollection.AddSingleton<IGameService, GameModule>();

            serviceCollection.AddSingleton<ITypeResolver, TypeResolver>();

            serviceCollection.AddSingleton<IAuthActionService, AuthActions>();
            serviceCollection.AddSingleton<IGameActionService, GameActions>();

            return new TypeRegistrar(serviceCollection);
        }
    }
}
