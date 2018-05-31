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
        CommandFunction function = new CommandFunction();
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
                        command = command.Replace('/', '\\');
                        cd.ExecuteChangeDirectory(command.ToLower());
                        break;
                    case "DIR":
                        command = command.Replace('/', '\\');
                        break;
                    case "CLS":
                        function.ClearScreen(command);
                        break;
                    case "HELP":
                        function.Help(command);
                        break;
                    case "COPY":
                        command = command.Replace('/', '\\');
                        break;
                    case "MOVE":
                        command = command.Replace('/', '\\');
                        break;
                    case "EXIT":
                        function.Exit(command);
                        break;
                    default:
                        break;
                }
            }
        }
    }
}
