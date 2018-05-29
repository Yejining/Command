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
            if (command.Length == 0)
            {
                Console.SetCursorPosition(0, Console.CursorTop - 1);
                ReadCommand();
                return;
            }

            if (Regex.IsMatch(command.ToLower(), Constant.VALID_CD))        // CD
            {
                CD(command);
                return;
            }
            else if (Regex.IsMatch(command.ToLower(), Constant.VALID_DIR))  // DIR
            {
                DIR(command.ToLower());
                return;
            }
            else if (Regex.IsMatch(command.ToLower(), Constant.VALID_COPY)) // COPY
            {
                CopyOrMove(command.ToLower());
                return;
            }
            else if (Regex.IsMatch(command.ToLower(), Constant.VALID_MOVE)) // MOVE
            {
                CopyOrMove(command.ToLower());
                return;
            }
            else if (Regex.IsMatch(command.ToLower(), Constant.VALID_EXIT)) // EXIT
            {
                return;
            }
            else if (Regex.IsMatch(command.ToLower(), Constant.VALID_CLS))  // CLS
            {
                Console.Clear();
                ReadCommand();
                return;
            }
            else
            {
                string newCommand = "";
                if (Regex.IsMatch(command, "&")) SplitBasedOnAmpersand(command, out command, out newCommand);
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

            string currentDirectory = Path.GetPathRoot(Environment.CurrentDirectory).ToString().ToLower();

            // 1. 경로로 들어가기
            if (words.Count == 0)
            {
                Console.WriteLine(folderPath.Path);
                ExecuteBasedOnAmpersand(newCommand);
                return;
            }
            else if (words[0] == currentDirectory.Remove(currentDirectory.Length - 1))
            {
                Console.WriteLine(folderPath.Path);
                ExecuteBasedOnAmpersand(newCommand);
                return;
            }

            // 시스템이 지정된 드라이브를 찾을 수 없거나
            // 파일 이름, 디렉터리 이름 또는 볼륨 레이블 구분이 잘못될 경우
            if (!IsAvaliableDrive(words[0]))
            {
                ExecuteBasedOnAmpersand(newCommand);
                return;
            }

            command = "";
            foreach(string word in words)
            {
                command += $"{word} ";
            }
            if (command.Length != 0) command.Remove(command.Length - 1);

            // 지정된 경로를 찾을 수 없는 경우
            if (!Directory.Exists(command))
            {
                Console.WriteLine(Constant.PATH_ERROR);
                ExecuteBasedOnAmpersand(newCommand);
                return;
            }

            // 경로 이동
            folderPath.SetCurrentDirectory(words[0]);
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

                ReadCommand();
                return;
            }

            // 명령어 삭제 및 경로 이전 필요없는 공백/문자(,;=) 삭제
            words = new List<string>(command.Split(Constant.SEPERATOR, StringSplitOptions.RemoveEmptyEntries));

            // 1. 명령어 삭제
            words = RemoveFunctionName(words, Constant.DIR);

            // 경로(.:\/)가 나오기 전 입력 무시 - 1
            regex = new Regex(Constant.NEGLIGIBLE_TO_DELETE);
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
            }

            // 경로(.:\/)가 나오기 전 입력 무시 - 2
            for (int index = indexToRemove.Count() - 1; index >= 0; index--)
            {
                words.RemoveAt(indexToRemove[index]);
            }

            command = "";
            foreach (string word in words)
            {
                command += $"{word} ";
            }
            if (command.Length != 0) command = command.Remove(command.Length - 1);

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
            ReadCommand();
            return;
        }

        public static string NormalizePath(string path)
        {
            return Path.GetFullPath(new Uri(path).LocalPath)
                       .TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar)
                       .ToUpperInvariant();
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

            GetFileInformation(command, out sourcePath, out sourceName, out destinationPath, out destinationName);

            // 파일 이름을 입력하지 않은 경우
            if (sourceName.Length == 0)
            {
                if (mode == Constant.MOVE && string.Compare(NormalizePath(sourcePath), NormalizePath(destinationPath)) != 0)
                {
                    Console.WriteLine("디렉터리를 이동했습니다.");
                }

                Console.WriteLine("파일을 지정하지 않았습니다.");

                ReadCommand();
                return;
            }

            // 존재하지 않는 경로에서 복사하려는 경우
            if (!Directory.Exists(sourcePath))
            {
                Console.WriteLine("지정된 경로를 찾을 수 없습니다.");
                ReadCommand();
                return;
            }

            // 존재하지 않는 파일을 복사하려는 경우
            if (!File.Exists(Path.Combine(sourcePath, sourceName)) && Directory.Exists(sourcePath))
            {
                Console.WriteLine("지정된 파일을 찾을 수 없습니다.");
                ReadCommand();
                return;
            }

            // 존재하지 않는 경로로 복사하는 경우
            if (!Directory.Exists(destinationPath))
            {
                Console.WriteLine("지정된 경로를 찾을 수 없습니다.");
                Console.WriteLine($"\t0개 파일이 {ment}되었습니다.");
                ReadCommand();
                return;
            }

            // 복사하려는 파일이 이미 경로에 존재하는 경우
            if (File.Exists(Path.Combine(destinationPath, destinationName)))
            {
                string question = $"{destinationName}을(를) 덮었쓰시겠습니까? (Yes/No/All): ";
                if (mode == Constant.MOVE) question = $"{Path.Combine(destinationPath, destinationName)}을(를) 덮었쓰시겠습니까? (Yes/No/All): ";

                string userAnswer = outputProcessor.GetAnswer(question);

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
                        userAnswer = outputProcessor.GetAnswer(question);
                    }
                }

                ReadCommand();
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
            ReadCommand();
            return;
        }

        /// <summary>
        /// 경로에서 파일 경로와 이름을 구별해 반환하는 메소드입니다.
        /// </summary>
        /// <param name="allPath">경로</param>
        /// <param name="fileName">파일 경로</param>
        /// <param name="directoryName">파일 이름</param>
        public void GetFileNameAndDirectoryName(string allPath, out string fileName, out string directoryName)
        {
            // fileName
            fileName = Path.GetFileName(allPath);

            Regex regex = new Regex(fileName.ToLower(), RegexOptions.RightToLeft);
            string filePath = regex.Replace(allPath.ToLower(), "", 1);
            
            // directoryName
            if (Directory.Exists(filePath))
            {
                if (string.Compare(filePath, "c:\\") == 0) directoryName = filePath;
                else directoryName = Path.GetDirectoryName(allPath);
            }
            else if (string.Compare(fileName, allPath) == 0)
            {
                directoryName = folderPath.Path;
            }
            else
            {
                directoryName = "";
            }

            // fileName, directoryName
            if (Directory.Exists(Path.Combine(directoryName, fileName)))
            {
                directoryName = Path.Combine(directoryName, fileName);
                fileName = "";
            }
        }

        /// <summary>
        /// copy 명령어와 move명령어 실행시 호출되는 메소드입니다.
        /// copy/move할 대상의 경로와 파일 이름,
        /// copy/move의 목적지 경로와 파일 이름을 반환합니다.
        /// </summary>
        /// <param name="command">명령어</param>
        /// <param name="sourcePath">복사/이동할 파일 경로</param>
        /// <param name="sourceName">복사/이동할 파일 이름</param>
        /// <param name="destinationPath">복사/이동 목적지 파일 경로</param>
        /// <param name="destinationName">복사/이동 목적지 파일 이름</param>
        public void GetFileInformation(string command, out string sourcePath, out string sourceName, out string destinationPath, out string destinationName)
        {
            List<string> words = new List<string>(command.Split(Constant.SEPERATOR, StringSplitOptions.RemoveEmptyEntries));
            words.RemoveAt(0);

            // 값 할당
            sourcePath = "";
            sourceName = "";
            destinationPath = "";
            destinationName = "";

            GetFileNameAndDirectoryName(words[0], out sourceName, out sourcePath);

            // destinationName, destinationPath
            if (words.Count == 1)   // 입력되지 않은 경우
            {
                destinationPath = folderPath.Path;
                destinationName = "";
            }
            else                    // 입력된 경우
            {
                GetFileNameAndDirectoryName(words[1], out destinationName, out destinationPath);
            }
        }

        /// <summary>
        /// 사용자로부터 입력받은 드라이브의 이름이 컴퓨터에 존재하는지 확인하는 메소드입니다.
        /// </summary>
        /// <param name="driveName">사용자가 입력한 드라이브명</param>
        /// <returns>사용자 입력 드라이브 존재여부</returns>
        public bool IsDriveExist(string driveName)
        {
            DriveInfo[] allDrives = DriveInfo.GetDrives();
            foreach (DriveInfo drive in allDrives)
            {
                if (string.Compare(driveName, drive.ToString(), true) == 0)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 사용자가 입력한 명령어에서 드라이브명이 실제로 존재하는지 검사합니다.
        /// </summary>
        /// <param name="command">사용자 입력 명령어</param>
        /// <returns>드라이브명 존재 여부</returns>
        public bool IsAvaliableDrive(string command)
        {
            string match = new Regex(Constant.DRIVE_DETECTER).Match(command).ToString();

            if (match.Length == 0) return true;

            if (!IsDriveExist(match))
            {
                if (match.Length != 1) Console.WriteLine(Constant.PATH_MISINPUT);
                else Console.WriteLine(Constant.UNFINDABLE_DRIVE);
                return false;
            }

            return true;
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
