using System;
using System.Diagnostics;
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

            gs.Start();

            waitForInput();
        }

        static object waitForInput() => Console.ReadLine();
    }
}
