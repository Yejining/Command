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
        InputProcessor inputProcessor = new InputProcessor();
        FolderPath folderPath;

        /// <summary>
        /// 프로그램이 실행될 때 호출되는 메소입니다.
        /// 파일 경로를 초기화하고 사용자로부터 명령문을 입력받습니다.
        /// </summary>
        public void RunCommand()
        {
            Console.Write(Constant.START);
            folderPath = FolderPath.GetInstance();
            ExecuteCommand(inputProcessor.GetCommand());
        }

        /// <summary>
        /// 사용자로부터 입력받은 명령어를 실행하는 메소드입니다.
        /// </summary>
        public void ExecuteCommand(string command)
        {
            switch (inputProcessor.Order(command))
            {
                case "":
                    Console.SetCursorPosition(0, Console.CursorTop - 1);
                    ExecuteCommand(inputProcessor.GetCommand());
                    return;
                case "CD":
                    CD(command);
                    return;
                case "DIR":
                    DIR(command.ToLower());
                    return;
                case "COPY":
                    CopyOrMove(command.ToLower());
                    return;
                case "MOVE":
                    CopyOrMove(command.ToLower());
                    return;
                case "EXIT":
                    return;
                case "CLS":
                    Console.Clear();
                    ExecuteCommand(inputProcessor.GetCommand());
                    return;
                default:
                    string newCommand = "";
                    if (Regex.IsMatch(command, "&")) Tool.SplitBasedOnAmpersand(command, out command, out newCommand);
                    Console.WriteLine($"\'{command}\'{Constant.NOT_EXCUTABLE}");
                    ExecuteBasedOnAmpersand(newCommand);
                    return;
            }
        }

        /// <summary>
        /// CD 명령어를 실행하는 메소드입니다.
        /// </summary>
        /// <param name="command">명령어</param>
        public void CD(string command)
        {
            List<string> words;
            string newCommand = "";

            // &가 나오는 경우
            if (Regex.IsMatch(command, "&")) Tool.SplitBasedOnAmpersand(command, out command, out newCommand);

            // words split(' ')
            words = new List<string>(command.Split(Constant.SEPERATOR, StringSplitOptions.RemoveEmptyEntries));

            // 명령어 삭제
            words = Tool.RemoveFunctionName(words, Constant.CD);

            // 경로 나오기 전 필요 없는 문자 및 공백 삭제
            words = Tool.CommandWithoutGarbageCharacter(words);

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

            string currentDirectory = Path.GetPathRoot(Environment.CurrentDirectory).ToString().ToLower();

            // 1. 경로로 들어가기
            if (words.Count == 0)
            {
                Console.WriteLine(folderPath.Path);
                ExecuteBasedOnAmpersand(newCommand);
                return;
            }
            else if (words[0] == currentDirectory)
            {
                Console.WriteLine(folderPath.Path);
                ExecuteBasedOnAmpersand(newCommand);
                return;
            }

            // 시스템이 지정된 드라이브를 찾을 수 없거나
            // 파일 이름, 디렉터리 이름 또는 볼륨 레이블 구분이 잘못될 경우
            if (!Tool.IsAvaliableDrive(words[0]))
            {
                ExecuteBasedOnAmpersand(newCommand);
                return;
            }

            command = Tool.Command(words);

            // 지정된 경로를 찾을 수 없는 경우
            if (!Directory.Exists(command))
            {
                Console.WriteLine(Constant.PATH_ERROR);
                ExecuteBasedOnAmpersand(newCommand);
                return;
            }

            // .과 관련한 예외처리
            string path = words[0];
            if (Regex.IsMatch(path, Constant.LIMIT_DOT))
            {
                ExecuteBasedOnAmpersand(newCommand);
                return;
            }

            // 상위 디렉터리로 이동
            if (Regex.IsMatch(path, Constant.UPPER_FOLDER))
            {
                MatchCollection matches = Regex.Matches(path, Constant.UPPER_FOLDER);
                folderPath.SetUpperDirectory(matches.Count);
                ExecuteBasedOnAmpersand(newCommand);
                return;
            }

            // 경로 이동
            folderPath.SetCurrentDirectory(path);
            ExecuteBasedOnAmpersand(newCommand);
            return;
        }

        /// <summary>
        /// DIR 명령어를 실행하는 메소드입니다.
        /// </summary>
        /// <param name="command">명령어</param>
        public void DIR(string command)
        {
            Regex regex;
            List<string> words;
            List<int> indexToRemove = new List<int>();

            // 명령어 바로 뒤에 큰 따옴표가 있는지 검사
            if (Regex.IsMatch(command, Constant.DETECT_DOUBLE_QUOTATION_AFTER_DIR))
            {
                regex = new Regex("[:\\\\]");
                string match = regex.Match(command.Remove(0, 4)).ToString();
                
                if (match.Length == 0)      // 상대경로
                {
                    Console.WriteLine($"\'{command}\'{Constant.NOT_EXCUTABLE}");
                }
                else if (match == "\\")     // \ 절대경로
                {
                    Console.WriteLine(Constant.PATH_ERROR);
                }
                else if (match == ":")      // 드라이브 절대경로
                {
                    regex = new Regex("^.*:\\\\[^\\.]");
                    match = regex.Match(command.Remove(0, 4)).ToString();
                    if (match.Length == 0) Console.WriteLine($"\'{command}\'{Constant.NOT_EXCUTABLE}");
                    else Console.WriteLine(Constant.PATH_MISINPUT);
                }

                ExecuteCommand(inputProcessor.GetCommand());
                return;
            }

            // 명령어 삭제 및 경로 이전 필요없는 공백/문자(,;=) 삭제
            words = new List<string>(command.Split(Constant.SEPERATOR, StringSplitOptions.RemoveEmptyEntries));

            // 1. 명령어 삭제
            words = Tool.RemoveFunctionName(words, Constant.DIR);

            // 경로 나오기 전 필요 없는 문자 및 공백 삭제
            words = Tool.CommandWithoutGarbageCharacter(words);

            command = Tool.Command(words);

            // 경로 나누기
            // 1. 띄어쓰기 기준으로 나누기
            // words = new List<string>(command.Split(Constant.SEPERATOR, StringSplitOptions.RemoveEmptyEntries));

            // 경로 나누기 생략
            // 경로가 0-1개만 입력된다고 가정
            // 경로에서 &가 등장하지 않는다고 가정
            string currentDirectory = Path.GetPathRoot(Environment.CurrentDirectory);
            currentDirectory = currentDirectory.Remove(currentDirectory.Length - 1).ToLower();

            string path;

            if (command.Length == 0 || command == currentDirectory)
            {
                path = folderPath.Path;
            }
            else
            {
                path = command;
            }

            outputProcessor.DIR(path);
            ExecuteCommand(inputProcessor.GetCommand());
            return;
        }

        /// <summary>
        /// copy명령어나 move명령어를 실행하는 메소드입니다.
        /// </summary>
        /// <param name="command">명령어</param>
        public void CopyOrMove(string command)
        {
            // 경로에 쌍따옴표가 없다고 가정
            int mode = Constant.COPY;
            if (Regex.IsMatch(command.ToLower(), Constant.VALID_MOVE)) mode = Constant.MOVE;
                
            List<string> words = new List<string>(command.Split(Constant.SEPERATOR, StringSplitOptions.RemoveEmptyEntries));
            words.RemoveAt(0);

            string sourcePath, sourceName;
            string destinationPath, destinationName;
            string ment = "복사";
            if (mode == Constant.MOVE) ment = "이동";

            Tool.GetFileInformation(command, out sourcePath, out sourceName, out destinationPath, out destinationName);

            // 파일 이름을 입력하지 않은 경우
            if (sourceName.Length == 0)
            {
                if (mode == Constant.MOVE && string.Compare(Tool.NormalizePath(sourcePath), Tool.NormalizePath(destinationPath)) != 0)
                {
                    Console.WriteLine("디렉터리를 이동했습니다.");
                }

                Console.WriteLine("파일을 지정하지 않았습니다.");

                ExecuteCommand(inputProcessor.GetCommand());
                return;
            }

            // 존재하지 않는 경로에서 복사하려는 경우
            if (!Directory.Exists(sourcePath))
            {
                Console.WriteLine("지정된 경로를 찾을 수 없습니다.");
                ExecuteCommand(inputProcessor.GetCommand());
                return;
            }

            // 존재하지 않는 파일을 복사하려는 경우
            if (!File.Exists(Path.Combine(sourcePath, sourceName)) && Directory.Exists(sourcePath))
            {
                Console.WriteLine("지정된 파일을 찾을 수 없습니다.");
                ExecuteCommand(inputProcessor.GetCommand());
                return;
            }

            // 존재하지 않는 경로로 복사하는 경우
            if (!Directory.Exists(destinationPath))
            {
                Console.WriteLine("지정된 경로를 찾을 수 없습니다.");
                Console.WriteLine($"\t0개 파일이 {ment}되었습니다.");
                ExecuteCommand(inputProcessor.GetCommand());
                return;
            }

            // 복사하려는 파일이 이미 경로에 존재하는 경우
            if (File.Exists(Path.Combine(destinationPath, destinationName)))
            {
                string question = $"{destinationName}을(를) 덮었쓰시겠습니까? (Yes/No/All): ";
                if (mode == Constant.MOVE) question = $"{Path.Combine(destinationPath, destinationName)}을(를) 덮었쓰시겠습니까? (Yes/No/All): ";

                string userAnswer = inputProcessor.GetAnswer(question);

                while (true)
                {
                    if (Regex.IsMatch(userAnswer, Constant.YES) || Regex.IsMatch(userAnswer, Constant.ALL))
                    {
                        if (mode == Constant.COPY)
                            File.Copy(Path.Combine(sourcePath, sourceName), Path.Combine(destinationPath, destinationName), true);
                        else if (mode == Constant.MOVE)
                            File.Move(Path.Combine(sourcePath, sourceName), Path.Combine(destinationPath, destinationName));

                        Console.WriteLine($"\t1개 파일이 {ment}되었습니다.");
                        break;
                    }
                    else if (Regex.IsMatch(userAnswer, Constant.NO))
                    {
                        Console.WriteLine($"\t0개 파일이 {ment}되었습니다.");
                        break;
                    }
                    else
                    {
                        userAnswer = inputProcessor.GetAnswer(question);
                    }
                }

                ExecuteCommand(inputProcessor.GetCommand());
                return;
            }

            // copy의 경우 복사할 파일 이름이 없는 경우
            // 해당 경로에 복사
            

            // 접근할 수 없는 구역으로 copy나 move를 시도할 경우

            // 일반
            if (mode == Constant.COPY)
                File.Copy(Path.Combine(sourcePath, sourceName), Path.Combine(destinationPath, destinationName), true);
            else if (mode == Constant.MOVE)
                File.Move(Path.Combine(sourcePath, sourceName), Path.Combine(destinationPath, destinationName));
            Console.WriteLine($"\t1개 파일이 {ment}되었습니다.");
            ExecuteCommand(inputProcessor.GetCommand());
            return;
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
                ExecuteCommand(inputProcessor.GetCommand());
            }
        }
    }
}
