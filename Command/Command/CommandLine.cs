using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Command.Data;
using Command.IOException;

namespace Command.Command
{
    class CommandLine
    {
        OutputProcessor outputProcessor = new OutputProcessor();
        FolderPath folderPath;

        /// <summary>
        /// 프로그램이 실행될 때 호출되는 메소입니다.
        /// 파일 경로를 초기화하고 사용자로부터 명령문을 입력받습니다.
        /// </summary>
        public void RunCommand()
        {
            Console.Write(Constant.START);
            folderPath = FolderPath.GetInstance();
            ReadCommand();
        }

        /// <summary>
        /// 사용자로부터 명령어를 입력받고 실행하는 메소드입니다.
        /// </summary>
        public void ReadCommand()
        {
            string command = outputProcessor.GetCommand();
            string[] words = command.Split(' ');

            if (words[0].Length != 0)
                if (words[0][words[0].Length - 1] == ';')
                    words[0] = words[0].Remove(words[0].Length - 1);

            switch (words[0].ToLower())
            {
                case "cls":
                    Console.Clear();
                    Console.WriteLine();
                    ReadCommand();
                    return;
                case "help":
                    outputProcessor.PrintHelp();
                    Console.WriteLine();
                    ReadCommand();
                    return;
                case "":
                    ReadCommand();
                    return;
                case "exit":
                    return;
                default:
                    break;
            }
        }
    }
}
