using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;

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
        /// 사용자로부터 명령어를 입력받고, 실행메소드를 호출하는 메소드입니다.
        /// </summary>
        public void ReadCommand()
        {
            ExecuteCommand(outputProcessor.GetCommand());
        }

        /// <summary>
        /// 사용자로부터 입력받은 명령어를 실행하는 메소드입니다.
        /// </summary>
        public void ExecuteCommand(string command)
        {
            string[] words = command.Split(Constant.SEPERATOR, StringSplitOptions.RemoveEmptyEntries);
            
            if (words.Count() == 0)
            {
                ReadCommand();
                return;
            }

            switch (words[0].ToLower())
            {
                case "cd":
                    CD(command);
                    ReadCommand();
                    return;
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
                case "exit":
                    return;
                default:
                    FilterValidOrder(command.ToLower());
                    return;
            }
        }

        /// <summary>
        /// ReadCommand()에서 실행되지 않은 유효한 명령어들을 찾아내는 메소드입니다.
        /// </summary>
        /// <param name="command">명령어</param>
        public void FilterValidOrder(string command)
        {
            // cd 명령어
            if (Regex.IsMatch(command, Constant.VALID_CD))
            {
                CD(command);
                ReadCommand();
                return;
            }
            else
            {
                string newCommand = "";
                if (Regex.IsMatch(command, "&")) SplitBasedOnAmpersand(command, out command, out newCommand);
                Console.WriteLine($"\'{command}\'{Constant.NOT_EXCUTABLE}");
                ExecuteBasedOnAmpersand(newCommand);
            }
        }

        /// <summary>
        /// CD 명령어를 실행하는 메소드입니다.
        /// </summary>
        /// <param name="command"></param>
        public void CD(string command)
        {
            List<string> words;
            List<int> indexToRemove = new List<int>();
            Regex regex = new Regex(Constant.NEGLIGIBLE_TO_DELETE);
            string newCommand = "";

            // &가 나오는 경우
            if (Regex.IsMatch(command, "&")) SplitBasedOnAmpersand(command, out command, out newCommand);
            words = new List<string>(command.Split(Constant.SEPERATOR, StringSplitOptions.RemoveEmptyEntries));

            // 명령어 삭제
            words = RemoveFunctionName(words, Constant.CD);

            // 경로(.:\/)가 나오기 전 입력 무시 - 1
            for (int index = 0; index < words.Count(); index++)
            {
                if (Regex.IsMatch(words[index], Constant.NEGLIGIBLE))
                {
                    indexToRemove.Add(index);
                }
                else if (Regex.IsMatch(words[index], Constant.CONSIDERABLE))
                {
                    words[index] = regex.Replace(words[index], "", 1);
                    break;
                }
                //else if (index != 0)
                //{
                //    Console.WriteLine(Constant.PATH_ERROR);
                //    return;
                //}
            }

            // 경로(.:\/)가 나오기 전 입력 무시 - 2
            for (int index = indexToRemove.Count() - 1; index >= 0; index--)
            {
                words.RemoveAt(indexToRemove[index]);
            }

            // 경로로 들어가기
            if (words.Count == 0)
            {
                Console.WriteLine(folderPath.Path);
                ExecuteBasedOnAmpersand(newCommand);
                return;
            }

            // unc경로인지 검사
            if (Regex.IsMatch(words[0], Constant.UNC_PATH_DETECTER))
            {
                Console.Write("\'");
                foreach (string word in words) Console.Write($"{word} ");
                Console.WriteLine("\b\'");
                Console.WriteLine(Constant.UNC_PATH_ERROR);
                ExecuteBasedOnAmpersand(newCommand);
                return;
            }

            // 1. 경로로 들어가기
            if (words.Count == 0)
            {
                Console.WriteLine(folderPath.Path);
                ExecuteBasedOnAmpersand(newCommand);
                return;
            }

            // 지정된 경로를 찾을 수 없는 경우


            // 경로 이동
            folderPath.SetCurrentDirectory(words[0]);
            ExecuteBasedOnAmpersand(newCommand);
        }

        /// <summary>
        /// 명령어를 '&' 기준으로 oldCommand와 newCommand로 나누는 메소드입니다.
        /// '&' 이전 명령어가 oldCommand, '&' 이후 명령어가 newCommand입니다.
        /// </summary>
        /// <param name="command">명령어</param>
        /// <param name="oldCommand">'&' 이전 명령어</param>
        /// <param name="newCommand">'&' 이후 명령어</param>
        public void SplitBasedOnAmpersand(string command, out string oldCommand, out string newCommand)
        {
            Regex regex;

            // '&' 이후 명령어
            regex = new Regex(Constant.DELETE_BEFORE_AMPERSAND);
            string match = regex.Match(command).ToString() + "&";
            newCommand = command.Replace(command, match);
            newCommand = command.Replace(match, "");

            // '&' 이전 명령어
            regex = new Regex(Constant.KEEP_BEFORE_AMPERSAND);
            oldCommand = regex.Replace(command, "", 1);
        }

        /// <summary>
        /// 사용자가 입력한 명령어에 '&'가 있을 경우 newCommand를 실행하고
        /// 그렇지 않은 경우 다음 명령어를 받는 메소드입니다.
        /// </summary>
        /// <param name="newCommand">'&' 뒤의 명령어</param>
        public void ExecuteBasedOnAmpersand(string newCommand)
        {
            if (newCommand.Length != 0)
            {
                ExecuteCommand(newCommand);
            }
            else
            {
                ReadCommand();
            }
        }

        /// <summary>
        /// 명령어에서 명령어 이름을 삭제하는 메소드입니다.
        /// </summary>
        /// <param name="words">명령어</param>
        /// <param name="functionName">명령어 이름</param>
        /// <returns>명령어 이름을 삭제한 명령어</returns>
        public List<string> RemoveFunctionName(List<string> words, string functionName)
        {
            words[0] = words[0].Remove(0, functionName.Length);
            if (words[0].Length == 0) words.RemoveAt(0);

            return words;
        }
    }
}
