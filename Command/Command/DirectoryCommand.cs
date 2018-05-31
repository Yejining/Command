using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;

using Command.Data;
using Command.IO;

namespace Command.Command
{
    class DirectoryCommand
    {
        DirectoryCommandException exception;

        public DirectoryCommand()
        {
            exception = new DirectoryCommandException(this);
        }

        public void ExecuteDIR(string command)
        {
            List<string> words = new List<string>(command.Split(Constant.SEPERATOR, StringSplitOptions.RemoveEmptyEntries));

            // dir 명령어 뒤에 다른 문자가 나오는 경우
            if (Regex.IsMatch(words[0], "^dir[^,^=^;^\\\\^\\.^\"]"))
            {
                Console.WriteLine($"\'{command}\'은(는) 내부 또는 외부 명령, 실행할 수 있는 프로그램, 또는\n배치 파일이 아닙니다.\n");
                return;
            }

            // command에서 "dir" 삭제
            words = exception.RemoveCommand(words);

            exception.AnalyzePath(words);
        }

        public void PrintDirectory(string path)
        {
            int folderCount, fileCount;
            string freeSpace = new DriveInfo(path).TotalFreeSpace.ToString();
            IOrderedEnumerable<SubFileDirectoryVO> subEntries = GetSubEntries(path, out folderCount, out fileCount);
            
            DriveInformation();
            Console.WriteLine($" {path} 디렉터리\n");

            long size = 0;
            foreach (SubFileDirectoryVO sub in subEntries)
            {
                Console.Write(sub.Date);
                if (sub.Size == 0)
                {
                    Console.Write("<DIR>");
                }
                else
                {
                    string subSize = String.Format("{0:n0}", sub.Size).PadLeft(10);
                    Console.SetCursorPosition(30, Console.CursorTop);
                    Console.Write(subSize);
                }
                Console.SetCursorPosition(41, Console.CursorTop);
                Console.WriteLine(sub.Name);
                size += sub.Size;
            }
            Console.SetCursorPosition(6, Console.CursorTop);
            Console.WriteLine($"{fileCount.ToString().PadLeft(10)}개 파일");
            Console.SetCursorPosition(6, Console.CursorTop);
            Console.Write($"{folderCount.ToString().PadLeft(10)}개 디렉터리");
            Console.SetCursorPosition(30, Console.CursorTop - 1);
            Console.WriteLine($"{String.Format("{0:n0}", size).PadLeft(15)} 바이트");
            Console.SetCursorPosition(30, Console.CursorTop);
            Console.WriteLine($"{String.Format("{0:n0}", long.Parse(freeSpace)).PadLeft(15)} 바이트 남음\n");
        }

        public void FileExistenceError(string path)
        {
            DriveInformation();

            Console.WriteLine($" {path} 디렉터리\n");
            Console.WriteLine("파일을 찾을 수 없습니다.\n");
        }

        public void DriveInformation()
        {
            string localDrive = Directory.GetDirectoryRoot(Directory.GetCurrentDirectory());

            // 볼륨 이름
            string volumeName = exception.GetVolumeName();
            if (volumeName.Length == 0)
                Console.WriteLine($" {localDrive.ToUpper()[0]} 드라이브의 볼륨에는 이름이 없습니다.");
            else
                Console.WriteLine($"볼륨 이름: {volumeName}");

            // 볼륨 일련 번호
            string volumeNumber = exception.GetVolumeNumber(localDrive[0]);
            Console.WriteLine($" 볼륨 일련 번호: {volumeNumber.Remove(4)}-{volumeNumber.Substring(4)}\n");
        }

        public List<SubFileDirectoryVO> GetDirectoryRoot(string path, out int folderCount)
        {
            List<SubFileDirectoryVO> subFileDirectoryVOList = new List<SubFileDirectoryVO>();
            DirectoryInfo dInfo = new DirectoryInfo(path);
            folderCount = 0;

            // 루트일 경우
            if (exception.IsRootDirectory(path))
                return subFileDirectoryVOList;

            subFileDirectoryVOList.Add(new SubFileDirectoryVO { Date = dInfo.LastWriteTime.ToString("yyyy-MM-dd tt hh:mm" + "    "), Name = ".", Size = 0 });
            subFileDirectoryVOList.Add(new SubFileDirectoryVO { Date = dInfo.Parent.LastWriteTime.ToString("yyyy-MM-dd tt hh:mm" + "    "), Name = "..", Size = 0 });
            folderCount += 2;

            return subFileDirectoryVOList;
        }

        public IOrderedEnumerable<SubFileDirectoryVO> GetSubEntries(string path, out int folderCount, out int fileCount)
        {
            List<SubFileDirectoryVO> subFileDirectoryVOList;
            DirectoryInfo dInfo = new DirectoryInfo(path);
            FileInfo info;
            string[] folders = Directory.GetDirectories(path);
            string[] files = Directory.GetFiles(path);
            folderCount = 0;
            fileCount = 0;

            subFileDirectoryVOList = GetDirectoryRoot(path, out folderCount);

            // 하위 폴더
            foreach (string subFolder in folders)
            {
                string folder = Path.Combine(path, subFolder);
                dInfo = new DirectoryInfo(folder);
                if ((dInfo.Attributes & FileAttributes.Hidden) != FileAttributes.Hidden)
                {
                    subFileDirectoryVOList.Add(new SubFileDirectoryVO { Date = dInfo.LastAccessTime.ToString("yyyy-MM-dd tt hh:mm" + "    "), Name = dInfo.Name, Size = 0 });
                    folderCount++;
                }
            }

            // 하위 파일
            foreach (string subFile in files)
            {
                string file = Path.Combine(path, subFile);
                info = new FileInfo(file);
                if ((info.Attributes & FileAttributes.Hidden) != FileAttributes.Hidden)
                {
                    subFileDirectoryVOList.Add(new SubFileDirectoryVO { Date = info.LastAccessTime.ToString("yyyy-MM-dd tt hh:mm" + "    "), Name = info.Name, Size = info.Length });
                    fileCount++;
                }
            }

            // 정렬
            IOrderedEnumerable<SubFileDirectoryVO> ordered = subFileDirectoryVOList.OrderBy(x => x.Name);

            return ordered;
        }
    }
}
