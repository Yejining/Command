using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;

using Command.Data;

namespace Command.Command
{
    class Tool
    {
        static FolderPath folderPath = FolderPath.GetInstance();

        /// <summary>
        /// 입력된 경로를 경로의 정석 모양대로 바꾸어주는 메소드입니다.
        /// </summary>
        /// <param name="path">경로</param>
        /// <returns>경로의 정석</returns>
        public static string NormalizePath(string path)
        {
            return Path.GetFullPath(new Uri(path).LocalPath)
                       .TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar)
                       .ToUpperInvariant();
        }

        /// <summary>
        /// 경로에서 파일 경로와 이름을 구별해 반환하는 메소드입니다.
        /// </summary>
        /// <param name="allPath">경로</param>
        /// <param name="fileName">파일 경로</param>
        /// <param name="directoryName">파일 이름</param>
        public static void GetFileNameAndDirectoryName(string allPath, out string fileName, out string directoryName)
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
        public static void GetFileInformation(string command, out string sourcePath, out string sourceName, out string destinationPath, out string destinationName)
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
        public static bool IsDriveExist(string driveName)
        {
            DriveInfo[] allDrives = DriveInfo.GetDrives();
            foreach (DriveInfo drive in allDrives)
            {
                if (driveName[0] ==drive.ToString().ToLower()[0])
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
        public static bool IsAvaliableDrive(string command)
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
        public static void SplitBasedOnAmpersand(string command, out string oldCommand, out string newCommand)
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
        /// 명령어에서 명령어 이름을 삭제하는 메소드입니다.
        /// </summary>
        /// <param name="words">명령어</param>
        /// <param name="functionName">명령어 이름</param>
        /// <returns>명령어 이름을 삭제한 명령어</returns>
        public static List<string> RemoveFunctionName(List<string> words, string functionName)
        {
            words[0] = words[0].Remove(0, functionName.Length);
            if (words[0].Length == 0) words.RemoveAt(0);

            return words;
        }

        /// <summary>
        /// Split했던 명령어를 하나로 합해 반환하는 메소드입니다.
        /// </summary>
        /// <param name="words">Split한 명령어 조각 리스트</param>
        /// <returns>명령어</returns>
        public static string Command(List<string> words)
        {
            string command = "";

            foreach (string word in words) command += $"{word} ";
            if (command.Length != 0) command.Remove(command.Length - 1);

            return command;
        }

        /// <summary>
        /// 명령어에서 경로가 나오기 전까지 필요 없는 문자와 공백을 모두 삭제해줍니다.
        /// </summary>
        /// <param name="words"></param>
        /// <returns></returns>
        public static List<string> CommandWithoutGarbageCharacter(List<string> words)
        {
            Regex regex = new Regex(Constant.NEGLIGIBLE_TO_DELETE);
            List<int> indexToRemove = new List<int>();

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

            return words;
        }
    }
}
