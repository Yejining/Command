using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;
using System.Management;
using System.Collections;

namespace Command.Command
{
    class DirectoryCommandException
    {
        DirectoryCommand dir;

        public DirectoryCommandException (DirectoryCommand dir)
        {
            this.dir = dir;
        }

        public List<string> RemoveCommand(List<string> words)
        {
            // dir 명령어만 있을 경우, 명령어 뒤에 무시 가능한 문자가 있을 경우
            if (Regex.IsMatch(words[0], "^dir$") || Regex.IsMatch(words[0], "^dir[,|=|;]*$"))
                words.RemoveAt(0);
            // dir 명령어 뒤에 무시 가능한 문자가 있을 경우
            else if (Regex.IsMatch(words[0], "^dir[,|=|;]*[^,|^=|^;]*"))
                words[0] = Regex.Replace(words[0], "^dir[,|=|;]*", "");
            // dir 명령어 뒤에 경로가 나올 경우
            else if (Regex.IsMatch(words[0], "^dir[\\\\|\\.]"))
                words[0] = words[0].Remove(0, 3);

            return words;
        }

        public void AnalyzePath(List<string> paths)
        {
            if (paths.Count == 0)
            {
                dir.PrintDirectory(Directory.GetCurrentDirectory());
                return;
            }

            string path = paths[0];

            switch (path[0])
            {
                case '\\':
                    StartWithSlash(path);
                    break;
                case '.':
                    StartWithDot(path);
                    break;
                default:
                    CheckPath(path);
                    break;
            }

            if (paths.Count != 1)
            {
                paths.RemoveAt(0);
                AnalyzePath(paths);
            }
        }

        public void StartWithSlash(string path)
        {
            if (Regex.IsMatch(path, "^\\\\$|^\\\\[^\\\\]") && (Directory.Exists(path) || File.Exists(path)))
            {
                    dir.PrintDirectory(path);
            }
            else if (Regex.IsMatch(path, "^\\\\$|^\\\\[^\\\\]"))
            {
                string newPath = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), path));

                if (Directory.Exists(Directory.GetParent(newPath).ToString()))
                    Console.WriteLine("파일을 찾을 수 없습니다.\n");
                else if (Directory.Exists(Directory.GetParent(Directory.GetParent(newPath).ToString()).ToString()))
                    Console.WriteLine("지정된 파일을 찾을 수 없습니다.\n");
                else
                    Console.WriteLine("지정된 경로를 찾을 수 없습니다.\n");
            }
            else if (Regex.IsMatch(path, "^\\\\\\\\$|^\\\\\\\\[^\\\\]"))
            {
                Console.WriteLine("파일 이름, 디렉터리 이름 또는 볼륨 레이블 구분이 잘못되었습니다.\n");
            }
            else
            {
                Console.WriteLine("지정된 경로가 잘못되었습니다.\n");
            }
        }

        public void StartWithDot(string path)
        {
            if (Regex.IsMatch(path, "^\\.$"))
            {
                dir.PrintDirectory(path);
            }
            else if (Regex.IsMatch(path, "^\\.$|^\\.[^\\.]|^\\.\\.$|^\\.\\.[^\\.]"))
            {
                string newPath = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), path));
                if (Directory.Exists(newPath) || File.Exists(newPath))
                    dir.PrintDirectory(newPath);
                else
                    Console.WriteLine("파일을 찾을 수 없습니다.\n");
            }
            else
            {
                Console.WriteLine("파일을 찾을 수 없습니다.\n");
            }
        }

        public void CheckPath(string path)
        {
            // ':'가 있는 경우
            if (Regex.IsMatch(path, ":"))
            {
                Regex driveFinder = new Regex(".*(?=:)");
                string drive = driveFinder.Match(path).ToString();

                // 드라이브 입력이 없는 경우
                if (drive.Length == 0)
                {
                    Console.WriteLine("파일을 찾을 수 없습니다.\n");
                    return;
                }
                // 입력된 드라이브가 존재하지 않는 경우
                else if (!IsExistDrive(drive))
                {
                    Console.WriteLine("지정된 경로를 찾을 수 없습니다.\n");
                    return;
                }

                // 입력된 드라이브가 존재하는 경우
                if (Regex.IsMatch(path, ".*(?=:):$"))
                {
                    dir.PrintDirectory(path);
                }
                else if (Regex.IsMatch(path, ".*(?=:):\\\\$|.*(?=:):\\\\.*"))
                {
                    if (Directory.Exists(path) || File.Exists(path))
                        dir.PrintDirectory(path);
                    else
                        Console.WriteLine("지정된 파일을 찾을 수 없습니다.\n");
                }
                else
                {
                    Console.WriteLine("파일을 찾을 수 없습니다.\n");
                }
            }
            // ':'가 없는 경우
            else
            {
                string newPath = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), path));

                if (Directory.Exists(newPath) || File.Exists(newPath))
                    dir.PrintDirectory(newPath);
                else
                    Console.WriteLine("파일을 찾을 수 없습니다.\n");
            }
        }

        public string GetVolumeNumber(char drive)
        {
            ManagementObject information = new ManagementObject(@"win32_logicaldisk.deviceid=""" + drive + @":""");
            information.Get();

            return information["VolumeSerialNumber"].ToString();
        }

        public string GetVolumeName()
        {
            string localDrive = Directory.GetDirectoryRoot(Directory.GetCurrentDirectory());

            foreach (DriveInfo drive in DriveInfo.GetDrives())
            {
                if (localDrive == drive.Name)
                    return drive.VolumeLabel;
            }

            return "";
        }

        public string GetHomeDirectory()
        {
            string path;

            path = Directory.GetParent(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)).FullName;
            if (Environment.OSVersion.Version.Major >= 6)
                path = Directory.GetParent(path).ToString();
            return path;
        }

        public bool IsExistDrive(string drive)
        {
            foreach (string localDrive in Directory.GetLogicalDrives())
            {
                if (drive == localDrive[0].ToString().ToLower())
                {
                    return true;
                }
            }

            return false;
        }
    }
}
