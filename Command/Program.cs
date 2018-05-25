using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Command.Command;

namespace Command
{
    class Program
    {
        static void Main(string[] args)
        {
            CommandLine commandLine = new CommandLine();

            commandLine.RunCommand();
        }
    }
}
