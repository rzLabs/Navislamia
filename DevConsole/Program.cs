using System;
using System.IO;
using Navislamia.Core;
using Navislamia.Events;

namespace DevConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            GameServer gs = new GameServer(Directory.GetCurrentDirectory(), "Configuration.json", null, null);

            // Subscribe to events
            gs.EventMgr.MessageOccured += EventMgr_MessageOccured;
            gs.EventMgr.ExceptionOccured += EventMgr_ExceptionOccured;

            Console.WriteLine("Begin GameServer initialization...");

            if (gs.Initialize())
            {
                Console.WriteLine("Initialization complete!");
            }

            waitForInput();
        }

        private static void EventMgr_ExceptionOccured(object sender, ExceptionArgs e)
        {
            Console.WriteLine($"An exception has occured!\n\n- nMessage: {e.Exception.Message}\n\nStack-Trace: {e.Exception.StackTrace}");
        }

        private static void EventMgr_MessageOccured(object sender, MessageArgs e)
        {
            Console.WriteLine(e.Message);
        }

        static object waitForInput() => Console.ReadLine();
    }
}
