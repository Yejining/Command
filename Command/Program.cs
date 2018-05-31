using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using Command.Command;

namespace Command
{
    class Program
    {
        static void Main(string[] args)
        {
            CommandExecute input = new CommandExecute();
            input.StartProgram();
        }
    }
}
