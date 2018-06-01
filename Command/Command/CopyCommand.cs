using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;

using Command.Data;

namespace Command.Command
{
    class CopyCommand
    {
        CommandException exception = new CommandException();

        public void Copy(string command)
        {
            List<string> words = new List<string>(command.Split(Constant.SEPERATOR, StringSplitOptions.RemoveEmptyEntries));
            words.RemoveAt(0);
            switch (words.Count)
            {
                case 2:
                    command = command.Remove(0, 5);
                    break;
                default:
                    Console.WriteLine("명령 구분이 올바르지 않습니다.\n");
                    return;
            }

            // 경로, 파일명 추출
            string sourcePath, sourceName;
            string destinationPath, destinationName;
            exception.GetFileInformation(command, out sourcePath, out sourceName, out destinationPath, out destinationName);

            // 에러 탐색
            if (!exception.IsValidCommand(sourcePath, sourceName, destinationPath, destinationName))
                return;

            // 덮어쓰는 경우
            if (exception.IsFileExist(destinationPath, destinationName))
            {
                Override(sourcePath, sourceName, destinationPath, destinationName);
                return;
            }

            // 복사
            File.Copy(Path.Combine(sourcePath, sourceName), Path.Combine(destinationPath, destinationName), true);
        }

        public void Override(string sourcePath, string sourceName, string destinationPath, string destinationName)
        {
            string question = $"{destinationName}을(를) 덮었쓰시겠습니까? (Yes/No/All): ";

            Console.Write(question);
            string answer = Console.ReadLine();

            while (true)
            {
                if (Regex.IsMatch(answer, Constant.YES) || Regex.IsMatch(answer, Constant.ALL))
                {
                    File.Copy(Path.Combine(sourcePath, sourceName), Path.Combine(destinationPath, destinationName), true);
                    Console.WriteLine("\t1개 파일이 복사되었습니다.\n");
                    break;
                }
                else if (Regex.IsMatch(answer, Constant.NO))
                {
                    Console.WriteLine($"\t0개 파일이 복사되었습니다.\n");
                    break;
                }
                else
                {
                    Console.Write(question);
                    answer = Console.ReadLine();
                }
            }
        }
    }
}
