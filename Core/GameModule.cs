using System;
using System.Threading.Tasks;
using System.Diagnostics;

using System.IO;
using System.Linq;
using System.Reflection;

using Configuration;
using Network;
using Notification;
using Scripting;
using Autofac;

namespace Game
{
    public class GameModule : Autofac.Module, IGameService
    {
        IConfigurationService configSVC;
        IScriptingService scriptSVC;
        INotificationService notificationSVC;
        INetworkService networkSVC;

        public GameModule() { }

        public GameModule(IConfigurationService configurationService, IScriptingService scriptService, INetworkService networkService, INotificationService notificationService)
        {
            configSVC = configurationService;
            scriptSVC = scriptService;
            networkSVC = networkService;
            notificationSVC = notificationService;
        }

        public int Start(string ip, int port, int backlog)
        {
            notificationSVC.WriteConsole("Starting game service...");

            return 0;
        }

        protected override void Load(ContainerBuilder builder)
        {
            var configServiceTypes = Directory.EnumerateFiles(Environment.CurrentDirectory)
                .Where(filename => filename.Contains("Modules") && filename.EndsWith("Game.dll"))
                .Select(filepath => Assembly.LoadFrom(filepath))
                .SelectMany(assembly => assembly.GetTypes()
                .Where(type => typeof(IGameService).IsAssignableFrom(type) && type.IsClass));

            foreach (var configServiceType in configServiceTypes)
                builder.RegisterType(configServiceType).As<IGameService>();
        }
    }
}
