using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;

using Command.IO;
using Command.Data;

namespace Command.Command
{
    class CommandExecute
    {
        InputProcessor input = new InputProcessor();
        ChangeDirectory cd = new ChangeDirectory();
        ChangeDirectoryException exception = new ChangeDirectoryException();
        CommandFunction function = new CommandFunction();
        DirectoryCommand dir = new DirectoryCommand();
        CopyCommand copy = new CopyCommand();
        MoveCommand move = new MoveCommand();
        private string command;

        public void StartProgram()
        {
            Directory.SetCurrentDirectory(exception.GetHomeDirectory());
            Console.WriteLine(Constant.START);
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
                    case "DRIVE":
                        cd.ChangeDrive(command[0]);
                        break;
                    case "CD":
                        command = command.Replace('/', '\\');
                        cd.ExecuteChangeDirectory(command.ToLower());
                        break;
                    case "DIR":
                        command = command.Replace('/', '\\');
                        dir.ExecuteDIR(command.ToLower());
                        break;
                    case "CLS":
                        function.ClearScreen(command);
                        break;
                    case "HELP":
                        function.Help(command);
                        break;
                    case "COPY":
                        command = command.Replace('/', '\\');
                        copy.Copy(command);
                        break;
                    case "MOVE":
                        command = command.Replace('/', '\\');
                        move.Move(command);
                        break;
                    case "EXIT":
                        if (function.Exit(command)) return;
                        break;
                    default:
                        Console.WriteLine($"\'{command}\'{Constant.NOT_EXCUTABLE}\n");
                        break;
                }
            }
        }
    }
}
