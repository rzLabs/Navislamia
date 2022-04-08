using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Spectre.Console.Cli;

namespace Navislamia.Command
{
    public interface ICommandService
    {
        public string Input { get; set; }

        public int Init(ITypeRegistrar registrar);

        public int Wait();

        public int Execute(string[] args);
    }
}
