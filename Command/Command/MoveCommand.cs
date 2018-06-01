using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;

using Command.Data;

namespace Command.Command
{
    class MoveCommand
    {
        CommandException exception = new CommandException();

        public void Move(string command)
        {
            List<string> words = new List<string>(command.Split(Constant.SEPERATOR, StringSplitOptions.RemoveEmptyEntries));
            words.RemoveAt(0);
            switch (words.Count)
            {
                case 1:
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
            if (!exception.IsValidMoveCommand(sourcePath, sourceName, destinationPath, destinationName))
                return;

            // 덮어쓰는 경우
            if (exception.IsOverwriteCase(sourcePath, sourceName, destinationPath, destinationName))
            {
                Overwrite(sourcePath, sourceName, destinationPath, destinationName);
                return;
            }

            // Move
            File.Move(Path.Combine(sourcePath, sourceName), Path.Combine(destinationPath, destinationName));
            Console.WriteLine("\t1개 파일을 이동했습니다.\n");
        }

        public void Overwrite(string sourcePath, string sourceName, string destinationPath, string destinationName)
        {
            string question = $"{Path.Combine(destinationPath, destinationName)}을(를) 덮었쓰시겠습니까? (Yes/No/All): ";

            Console.Write(question);
            string answer = Console.ReadLine();

            while (true)
            {
                if (Regex.IsMatch(answer, Constant.YES) || Regex.IsMatch(answer, Constant.ALL))
                {
                    File.Copy(Path.Combine(sourcePath, sourceName), Path.Combine(destinationPath, destinationName), true);
                    File.Delete(Path.Combine(sourcePath, sourceName));
                    Console.WriteLine("\t1개 파일을 이동했습니다.\n");
                    break;
                }
                else if (Regex.IsMatch(answer, Constant.NO))
                {
                    Console.WriteLine("\t0개 파일을 이동했습니다.\n");
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
