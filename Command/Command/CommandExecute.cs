using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;

using Command.IO;

namespace Command.Command
{
    class CommandExecute
    {
        InputProcessor input = new InputProcessor();
        ChangeDirectory cd = new ChangeDirectory();
        ChangeDirectoryException exception = new ChangeDirectoryException();
        private string command;

        public void StartProgram()
        {
            Directory.SetCurrentDirectory(exception.GetHomeDirectory());
            ExecuteCommand();
        }

        public void ExecuteCommand()
        {
            while (true)
            {
                command = input.GetCommand();

                switch (input.Function(command))
                {
                    case "":
                        break;
                    case "CD":
                        cd.ExecuteChangeDirectory(command.ToLower());
                        break;
                    case "DIR":
                        break;
                    case "CLS":
                        break;
                    case "HELP":
                        break;
                    case "COPY":
                        break;
                    case "MOVE":
                        break;
                    case "EXIT":
                        break;
                    default:
                        break;
                }
            }
        }
    }
}
