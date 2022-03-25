using System;
using System.Diagnostics;
using System.IO;
using Navislamia.Core;

using Serilog;
using Serilog.Core;


namespace DevConsole
{
    class Program
    {
        static LoggingLevelSwitch logLevel = new LoggingLevelSwitch();

        static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                            .MinimumLevel.ControlledBy(logLevel)
                            .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level}] {Message:lj}{NewLine}{Exception}", theme: Serilog.Sinks.SystemConsole.Themes.AnsiConsoleTheme.Code)
                            .WriteTo.File(".\\Logs\\log-.txt", rollingInterval: RollingInterval.Day, outputTemplate: "[{Timestamp:HH:mm:ss} {Level}] {Message:lj}{NewLine}{Exception}")
                            .CreateLogger();

            GameServer gs = new GameServer(Directory.GetCurrentDirectory(), "Configuration.json", null, null);
            CommandUtility cmdUtil = new CommandUtility();

            gs.Start();
            cmdUtil.Wait("Waiting for input...");
        }

        public static void Shutdown(int seconds = 60)
        {
            // TODO: shut down the gameserver
        }

        static object waitForInput() => Console.ReadLine();
    }
}
