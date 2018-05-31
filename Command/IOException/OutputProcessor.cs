using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Management;
using System.Collections;
using System.Text;
using System.Security.AccessControl;
using System.Text.RegularExpressions;

using Command.Data;

namespace Command.IOException
{
    class OutputProcessor
    {
        FolderPath folderPath;

        /// <summary>
        /// OutputProcessor의 생성자 메소드입니다.
        /// 싱글톤 패턴인 folderPath의 객체를 불러옵니다.
        /// </summary>
        public OutputProcessor()
        {
            folderPath = FolderPath.GetInstance();
        }

        /// <summary>
        /// help 명령어 실행시 도움말을 출력해주는 메소드입니다.
        /// </summary>
        public void PrintHelp()
        {
            foreach (string help in Constant.HELP)
            {
                Console.WriteLine(help);
            }
        }

        /// <summary>
        /// dir 명령어 실행시 path의 디렉터리 정보를 출력해주는 메소드입니다.
        /// </summary>
        /// <param name="path">폴더 또는 파일 경로</param>
        public void DIR(string path)
        {
            //ArrayList hardDriveDetails = new ArrayList();
            //DriveInfo drive = new DriveInfo(path);
            //DirectoryInfo dInfo = new DirectoryInfo(path);
            //DirectorySecurity dSecurity = dInfo.GetAccessControl();
            //FileInfo info;

            //// 드라이브 볼륨
            //if (drive.VolumeLabel.Length == 0) Console.WriteLine($" {path[0].ToString().ToUpper()} 드라이브의 볼륨에는 이름이 없습니다.");
            //else Console.WriteLine(drive.VolumeLabel);

            //// 드라이브 일련 번호
            //ManagementObjectSearcher moSearcher = new ManagementObjectSearcher("SELECT * FROM Win32_DiskDrive");
            //foreach (ManagementObject wmi_HD in moSearcher.Get())
            //{
            //    string serialNo = wmi_HD["SerialNumber"].ToString();
            //    Console.WriteLine($" 드라이브 일련 번호 : {serialNo}");
            //}

            //// 디렉터리
            //Console.WriteLine($"\n {path} 디렉터리\n");

            //// 정보
            //List<SubFileDirectoryVO> subFileDirectoryVOList = new List<SubFileDirectoryVO>();
            //string[] folders = Directory.GetDirectories(path);
            //string[] files = Directory.GetFiles(path);
            //int folderCount = 0;
            //int fileCount = 0;

            //foreach (string subFolder in folders)
            //{
            //    string folder = Path.Combine(path, subFolder);
            //    dInfo = new DirectoryInfo(folder);
            //    if ((dInfo.Attributes & FileAttributes.Hidden) != FileAttributes.Hidden)
            //    {
            //        subFileDirectoryVOList.Add(new SubFileDirectoryVO { Date = dInfo.LastAccessTime.ToShortDateString(), Time = dInfo.LastAccessTime.ToShortTimeString(), Name = dInfo.Name, Size = 0 });
            //        folderCount++;
            //    }
            //}

            //foreach (string subFile in files)
            //{
            //    string file = Path.Combine(path, subFile);
            //    info = new FileInfo(file);
            //    if ((info.Attributes & FileAttributes.Hidden) != FileAttributes.Hidden)
            //    {
            //        subFileDirectoryVOList.Add(new SubFileDirectoryVO { Date = info.LastAccessTime.ToShortDateString(), Time = info.LastAccessTime.ToShortTimeString(), Name = info.Name, Size = info.Length });
            //        fileCount++;
            //    }
            //}

            //// current directory, parent directory 추가
            //info = new FileInfo(path);
            //subFileDirectoryVOList.Add(new SubFileDirectoryVO { Date = info.LastAccessTime.ToShortDateString(), Time = info.LastAccessTime.ToShortTimeString(), Name = ".", Size = 0 });
            //subFileDirectoryVOList.Add(new SubFileDirectoryVO { Date = info.LastAccessTime.ToShortDateString(), Time = info.LastAccessTime.ToShortTimeString(), Name = "..", Size = 0 });
            //folderCount += 2;

            //IOrderedEnumerable<SubFileDirectoryVO> ordered = subFileDirectoryVOList.OrderBy(x => x.Name);
            //// 출력
            //long size = 0;
            //foreach (SubFileDirectoryVO sub in ordered)
            //{
            //    string time = sub.Time; ;
            //    if (sub.Time.Remove(0, 4).ToString()[0] == ':') time = sub.Time.Insert(3, "0");

            //    Console.Write($"{sub.Date}  {time}    ");
            //    if (sub.Size == 0)
            //    {
            //        Console.Write("<DIR>");
            //    }
            //    else
            //    {
            //        string subSize = sub.Size.ToString().PadLeft(10);
            //        Console.SetCursorPosition(30, Console.CursorTop);
            //        Console.Write(subSize);
            //    }
            //    Console.SetCursorPosition(41, Console.CursorTop);
            //    Console.WriteLine(sub.Name);
            //    size += sub.Size;
            //}
            //Console.SetCursorPosition(6, Console.CursorTop);
            //Console.WriteLine($"{fileCount.ToString().PadLeft(10)}개 파일");
            //Console.SetCursorPosition(6, Console.CursorTop);
            //Console.Write($"{folderCount.ToString().PadLeft(10)}개 디렉터리");
            //Console.SetCursorPosition(30, Console.CursorTop - 1);
            //Console.WriteLine($"{size.ToString().PadLeft(15)} 바이트");
            //Console.SetCursorPosition(30, Console.CursorTop);
            //Console.Write($"{drive.TotalFreeSpace.ToString().PadLeft(15)} 바이트 남음");
        }


    }
}
