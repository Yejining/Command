using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

using Command.IO;

namespace Command.Command
{
    class CommandFunction
    {
        OutputProcessor output = new OutputProcessor();

        public void ClearScreen(string command)
        {
            // 명령어 가공
            string newCommand = command.Remove(0, 3);

            if (newCommand.Length == 0)
            {
                Console.Clear();
                Console.WriteLine();
                return;
            }

            switch (newCommand[0])
            {
                case '/':
                    Console.WriteLine("명령 구분이 올바르지 않습니다.\n");
                    return;
                case '(':
                case ' ':
                    Console.Clear();
                    Console.WriteLine();
                    return;
                default:
                    break;
            }

            if (!Regex.IsMatch(newCommand, "[^,^.^+^=^:^;^\\\\]"))
                Console.WriteLine();
            else
                Console.WriteLine($"\'{command}\'은(는) 내부 또는 외부 명령, 실행할 수 있는 프로그램, 또는\n배치 파일이 아닙니다.");
        }

        public void Help(string command)
        {
            // 명령어 가공
            string newCommand = command.Remove(0, 4);

            // 명령어 뒤에 다른 문자가 없는 경우
            if (newCommand.Length == 0)
            {
                output.PrintHelp();
                return;
            }

            // 명령어 뒤에 다른 문자가 있는 경우
            if (!Regex.IsMatch(newCommand, "[^&^\"^\\s]"))
                output.PrintHelp();
            else
                Console.WriteLine($"\'{command}\'은(는) 내부 또는 외부 명령, 실행할 수 있는 프로그램, 또는\n배치 파일이 아닙니다.");

            return;
        }

        public bool Exit(string command)
        {
            // 명령어 가공
            string newCommand = command.Remove(0, 4);

            // 명령어 뒤에 다른 문자가 없는 경우
            if (newCommand.Length == 0)
                return true;

            // 명령어 뒤에 다른 문자가 있는 경우
            if (newCommand[0] == '(' || newCommand[0] == ' ' || !Regex.IsMatch(newCommand, "[^&^+^=^\\\\^,^.^/]"))
                return true;
            else
                Console.WriteLine($"\'{command}\'은(는) 내부 또는 외부 명령, 실행할 수 있는 프로그램, 또는\n배치 파일이 아닙니다.");

            return false;
        }
    }
}
