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

            GetFileNameAndDirectoryPath(words[0], out sourceName, out sourcePath);
            GetFileNameAndDirectoryPath(words[1], out destinationName, out destinationPath);
        }

        public static void GetFileNameAndDirectoryPath(string allPath, out string fileName, out string directoryPath)
        {
            if (Regex.IsMatch(allPath, "\\\\"))
            {
                fileName = Path.GetFileName(allPath);
                Regex regex = new Regex(fileName.ToLower(), RegexOptions.RightToLeft);
                directoryPath = Path.GetFullPath(regex.Replace(allPath.ToLower(), "", 1));
            }
            else
            {
                string path = Path.Combine(Directory.GetCurrentDirectory(), allPath);
                
                if (File.Exists(path))
                {
                    directoryPath = Directory.GetCurrentDirectory();
                    fileName = allPath;
                    return;
                }
                
                path = path.Replace(Path.GetFileName(path), "");
                if (Directory.Exists(path))
                {
                    directoryPath = path;
                    fileName = "";
                }
                else
                {
                    directoryPath = "";
                    fileName = "";
                }
            }
        }

        public bool IsValidCommand(string sourcePath, string sourceName, string destinationPath, string destinationName)
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
            else if (sourcePath == destinationPath && (sourceName == destinationName || destinationName == ""))
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

        public bool IsFileExist(string destinationPath, string destinationName)
        {
            if (File.Exists(Path.GetFullPath(Path.Combine(destinationPath, destinationName))))
                return true;
            else
                return false;
        }
    }
}
