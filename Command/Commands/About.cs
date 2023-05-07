using Navislamia.Notification;
using Spectre.Console;
using Spectre.Console.Cli;
using System.Diagnostics.CodeAnalysis;

namespace Navislamia.Command.Commands
{
    public interface IAbout
    {
        public void Print();
    }

    public class AboutPrinter : IAbout
    {
        INotificationModule notificaitonSVC;

        public AboutPrinter(INotificationModule notificationModule) => notificaitonSVC = notificationModule;

        public void Print()
        {

            var mainTbl = new Table().Border(TableBorder.Double).BorderColor(Color.Orange3);

            mainTbl.AddColumn("Description");
            mainTbl.AddColumn("Contributors and Advisors");
            mainTbl.AddColumns("Third Party Software");
            mainTbl.AddRow("Navislamia is a .NET 5 reimplementation of the Arcadia Framework", "- Aodai\n- iSmokeDrow\n- Glandu2\n- Graphos\n- Mango\n- Sandro\n- Pyrok\n- Nexitis", "- Moonsharp\n- Newtonsoft.JSON\n- Serilog\n- Spectre.Console\n- Dapper");

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
