using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;

using Command.Data;

namespace Command.Command
{
    class ChangeDirectory
    {
        ChangeDirectoryException exception = new ChangeDirectoryException();

        public void ExecuteChangeDirectory(string command)
        {
            // 명령어 가공
            command = command.Remove(0, 2);
            command = command.Replace("\\\\", "\\");

            // 명령어 바로 다음에 공백이 없는 경우 예외처리
            if (command[0] != ' ' && !exception.SpaceAbsense(command, out command))
                return;

            List<string> words = new List<string>(command.Split(Constant.SEPERATOR, StringSplitOptions.RemoveEmptyEntries));
            switch (words.Count)
            {
                case 0:
                    Console.WriteLine($"{Directory.GetCurrentDirectory()}\n");
                    return;
                case 1:
                    CheckPathValidity(words[0], "");
                    return;
                case 2:
                    CheckPathValidity(words[0], words[1]);
                    return;
                default:
                    Console.WriteLine("지정된 경로를 찾을 수 없습니다.\n");
                    return;
            }
        }

        public void CheckPathValidity(string path1, string path2)
        {
            bool result;

            switch (path1[0])
            {
                case '.':
                    result = exception.StartWithDot(path1, out path1);
                    break;
                default:
                    result = exception.CheckPath(path1 + path2);
                    break;
            }

            if (!result) return;

            // path2가 .이 아닌 다른 문자로 이루어진 경우 검사
            if (path2 != null && Regex.IsMatch(path2, "[^\\.^\\s]"))
            {
                Console.WriteLine("지정된 경로를 찾을 수 없습니다.\n");
                return;
            }
        }
    }
}
