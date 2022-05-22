using Notification;
using Spectre.Console;
using Spectre.Console.Cli;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Navislamia.Command.Commands
{
    public interface IAbout
    {
        public void Print();
    }

    public class AboutPrinter : IAbout
    {
        INotificationService notificaitonSVC;

        public AboutPrinter(INotificationService notificationService) => notificaitonSVC = notificationService;

        public void Print()
        {

            var mainTbl = new Table().Border(TableBorder.Double).BorderColor(Color.Orange3);

            mainTbl.AddColumn("Description");
            mainTbl.AddColumn("Contributors and Advisors");
            mainTbl.AddColumns("Third Party Software");
            mainTbl.AddRow("Navislamia is a .NET 5 reimplementation of the Arcadia Framework", "- Aodai\n- iSmokeDrow\n- Glandu2\n- Graphos\n- Mango\n- Sandro\n- Pyrok", "- Moonsharp\n- Newtonsoft.JSON\n- Serilog\n- Spectre.Console\n- Dapper");

            mainTbl.Border = TableBorder.Ascii;

            notificaitonSVC.Write(mainTbl);
            notificaitonSVC.WriteString("\n");
        }
    }

    public class About : Command<About.Settings>
    {
        IAbout _about;

        public class Settings : CommandSettings
        {

        }

        public About(IAbout about)
        {
            _about = about;
        }


        public override int Execute([NotNull] CommandContext context, [NotNull] Settings settings)
        {
            _about.Print();

            return 0;
        }
    }
}
