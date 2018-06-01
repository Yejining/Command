using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;

using Command.Data;

namespace Command.IO
{
    class InputProcessor
    {
        public string GetCommand()
        {
            Console.Write($"{Directory.GetCurrentDirectory()}>");
            return RefineCommand(Console.ReadLine());
        }

        public string RefineCommand(string command)
        {
            // 명령어 입력 전 공백 제거
            Regex space = new Regex("^\\s*(?![(\\s)])");
            return space.Replace(command, "");
        }

        public string Function(string command)
        {
            if (command.Length == 0) return "";
            else if (Regex.IsMatch(command, Constant.VALID_CD, RegexOptions.IgnoreCase)) return "CD";
            else if ((Regex.IsMatch(command, Constant.VALID_DIR, RegexOptions.IgnoreCase))) return "DIR";
            else if (Regex.IsMatch(command, Constant.VALID_CLS, RegexOptions.IgnoreCase)) return "CLS";
            else if (Regex.IsMatch(command, Constant.VALID_HELP, RegexOptions.IgnoreCase)) return "HELP";
            else if ((Regex.IsMatch(command, Constant.VALID_COPY, RegexOptions.IgnoreCase))) return "COPY";
            else if (Regex.IsMatch(command, Constant.VALID_MOVE, RegexOptions.IgnoreCase)) return "MOVE";
            else if (Regex.IsMatch(command, Constant.VALID_EXIT, RegexOptions.IgnoreCase)) return "EXIT";
            else if (Regex.IsMatch(command, Constant.VALID_CHANGE_DRIVE, RegexOptions.IgnoreCase)) return "DRIVE";
            else return "default";
        }
    }
}
