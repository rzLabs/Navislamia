using Navislamia.Command;
using Spectre.Console;
using System;
using System.Threading.Tasks;

namespace DevConsole
{
    class Program
    {
        static void Main(string[] args)
        {

            var container = ContainerConfig.Configure();
            var resolver = container.Build();      
            var app = resolver.Resolve(typeof(IApplication)) as IApplication;

            app.Run();

            var cli = resolver.Resolve(typeof(ICommandService)) as ICommandService;

            if (cli.Init(container) > 0)
                return;

            while (true)
                if (cli.Wait() == 0)
                    break;
        }
    }
}
