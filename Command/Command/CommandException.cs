using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;

using Command.Data;

namespace Command.Command
{
    class CommandException
    {
        public void GetFileInformation(string command, out string sourcePath, out string sourceName, out string destinationPath, out string destinationName)
        {
            List<string> words = new List<string>(command.Split(Constant.SEPERATOR, StringSplitOptions.RemoveEmptyEntries));

            // 값 할당
            sourcePath = "";
            sourceName = "";
            destinationPath = "";
            destinationName = "";

            // source
            GetFileNameAndDirectoryPath(words[0], out sourceName, out sourcePath);

            // destination
            if (words.Count != 1)
                GetFileNameAndDirectoryPath(words[1], out destinationName, out destinationPath);
            else
                destinationPath = Path.GetFullPath(Directory.GetCurrentDirectory());

            if (destinationName.Length == 0)
                destinationName = sourceName;
        }

        public static void GetFileNameAndDirectoryPath(string allPath, out string fileName, out string directoryPath)
        {
            fileName = "";
            directoryPath = "";

            if (Directory.Exists(allPath) && !File.Exists(allPath))
            {
                directoryPath = Path.GetFullPath(allPath);
                fileName = "";
            }
            else if (Regex.IsMatch(allPath, "\\\\"))
            {
                fileName = Path.GetFileName(allPath);
                Regex regex = new Regex(fileName.ToLower(), RegexOptions.RightToLeft);
                directoryPath = Path.GetFullPath(regex.Replace(allPath.ToLower(), "", 1));
                directoryPath = directoryPath.Remove(directoryPath.Length - 1);
            }
            else
            {
                directoryPath = Directory.GetCurrentDirectory();
                fileName = allPath;
            }
        }

        public bool IsValidCopyCommand(string sourcePath, string sourceName, string destinationPath, string destinationName)
        {
            sourcePath = Path.GetFullPath(sourcePath);
            destinationPath = Path.GetFullPath(destinationPath);
            
            // sourcePath가 유효하지 않은 경로일 경우
            if (!Directory.Exists(sourcePath))
            {
                Console.WriteLine("지정된 경로를 찾을 수 없습니다.\n");
                return false;
            }
            // sourceName이 존재하지 않는 파일명일 경우
            else if (Directory.Exists(sourcePath) && !File.Exists(Path.Combine(sourcePath, sourceName)))
            {
                Console.WriteLine("지정된 파일을 찾을 수 없습니다.\n");
                return false;
            }
            // 같은 파일로 복사하려는 경우
            else if (sourcePath == destinationPath && (sourceName.ToLower() == destinationName.ToLower() || destinationName == ""))
            {
                Console.WriteLine("같은 파일로 복사할 수 없습니다.\n\t0개 파일이 복사되었습니다.\n");
                return false;
            }
            // destnationPath가 유효하지 않은 경로일 경우
            else if (!Directory.Exists(destinationPath))
            {
                Console.WriteLine("지정된 경로를 찾을 수 없습니다.\n\t0개 파일이 복사되었습니다.\n");
                return false;
            }

            return true;
        }

        public bool IsValidMoveCommand(string sourcePath, string sourceName, string destinationPath, string destinationName)
        {
            sourcePath = Path.GetFullPath(sourcePath);
            destinationPath = Path.GetFullPath(destinationPath);

            // sourcePath가 유효하지 않은 경로일 경우
            if (!Directory.Exists(sourcePath))
            {
                Console.WriteLine("지정된 경로를 찾을 수 없습니다.\n");
                return false;
            }
            // sourceName이 존재하지 않는 파일명일 경우
            else if (Directory.Exists(sourcePath) && !File.Exists(Path.Combine(sourcePath, sourceName)))
            {
                Console.WriteLine("지정된 파일을 찾을 수 없습니다.\n");
                return false;
            }
            // destnationPath가 유효하지 않은 경로일 경우
            else if (!Directory.Exists(destinationPath))
            {
                Console.WriteLine("지정된 경로를 찾을 수 없습니다.\n\t0개 파일이 이동되었습니다.\n");
                return false;
            }

            return true;
        }

        public bool IsFileExist(string destinationPath, string destinationName)
        {
            if (File.Exists(Path.GetFullPath(Path.Combine(destinationPath, destinationName))))
                return true;
            else
                return false;
        }

        public bool IsOverwriteCase(string sourcePath, string sourceName, string destinationPath, string destinationName)
        {
            sourcePath = Path.GetFullPath(sourcePath);
            destinationPath = Path.GetFullPath(destinationPath);

            // 같은 경로, 다른 파일로 이동하는 경우
            if (sourcePath == destinationPath && sourceName.ToLower() != destinationName.ToLower() && IsFileExist(destinationPath, destinationName))
                return true;
            // 다른 경로로 이동하면서 파일이 존재하는 경우
            else if (sourcePath != destinationPath && IsFileExist(destinationPath, destinationName))
                return true;

            return false;
        }
    }
}
