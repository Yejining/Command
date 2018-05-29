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
        InputProcessor inputProcessor = new InputProcessor();
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
        /// 사용자로부터 명령어를 입력받고 리턴해주는 메소드입니다.
        /// </summary>
        /// <returns>명령어</returns>
        public string GetCommand()
        {
            Console.Write("\n" + folderPath.PathToUse());
            return inputProcessor.CommandFromUser();
        }

        /// <summary>
        /// 사용자에게 질문을 출력해주고 답을 얻어 반환하는 메소드입니다.
        /// </summary>
        /// <param name="question"></param>
        /// <returns>사용자 답변</returns>
        public string GetAnswer(string question)
        {
            Console.Write(question);
            return inputProcessor.CommandFromUser();
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
            ArrayList hardDriveDetails = new ArrayList();
            DriveInfo drive = new DriveInfo(path);
            DirectoryInfo dInfo = new DirectoryInfo(path);
            DirectorySecurity dSecurity = dInfo.GetAccessControl();

            // 드라이브 볼륨
            if (drive.VolumeLabel.Length == 0) Console.WriteLine($" {path[0].ToString().ToUpper()} 드라이브의 볼륨에는 이름이 없습니다.");
            else Console.WriteLine(drive.VolumeLabel);

            // 드라이브 일련 번호
            ManagementObjectSearcher moSearcher = new ManagementObjectSearcher("SELECT * FROM Win32_DiskDrive");
            foreach (ManagementObject wmi_HD in moSearcher.Get())
            {
                string serialNo = wmi_HD["SerialNumber"].ToString();
                Console.WriteLine($" 드라이브 일련 번호 : {serialNo}");
            }

            // 디렉터리
            Console.WriteLine($"\n {path} 디렉터리\n");

            // 정보
            List<SubFileDirectoryVO> subFileDirectoryVOList = new List<SubFileDirectoryVO>();
            string[] folders = Directory.GetDirectories(path);
            string[] files = Directory.GetFiles(path);
            int folderCount = 0;
            int fileCount = 0;

            foreach (string subFolder in folders)
            {
                string folder = Path.Combine(path, subFolder);
                DirectoryInfo info = new DirectoryInfo(folder);
                if ((info.Attributes & FileAttributes.Hidden) != FileAttributes.Hidden)
                {
                    subFileDirectoryVOList.Add(new SubFileDirectoryVO { Date = info.LastAccessTime.ToShortDateString(), Time = info.LastAccessTime.ToShortTimeString(), Name = info.Name, Size = 0 });
                    folderCount++;
                }
            }

            foreach (string subFile in files)
            {
                string file = Path.Combine(path, subFile);
                FileInfo info = new FileInfo(file);
                if ((info.Attributes & FileAttributes.Hidden) != FileAttributes.Hidden)
                {
                    subFileDirectoryVOList.Add(new SubFileDirectoryVO { Date = info.LastAccessTime.ToShortDateString(), Time = info.LastAccessTime.ToShortTimeString(), Name = info.Name, Size = info.Length });
                    fileCount++;
                }
            }

            IOrderedEnumerable<SubFileDirectoryVO> ordered = subFileDirectoryVOList.OrderBy(x => x.Name);
            // 출력
            long size = 0;
            foreach (SubFileDirectoryVO sub in ordered)
            {
                string time = sub.Time; ;
                if (sub.Time.Remove(0, 4).ToString()[0] == ':') time = sub.Time.Insert(3, "0");

                Console.Write($"{sub.Date}  {time}    ");
                if (sub.Size == 0)
                {
                    Console.Write("<DIR>");
                }
                else
                {
                    string subSize = sub.Size.ToString().PadLeft(10);
                    Console.SetCursorPosition(30, Console.CursorTop);
                    Console.Write(subSize);
                }
                Console.SetCursorPosition(41, Console.CursorTop);
                Console.WriteLine(sub.Name);
                size += sub.Size;
            }
            Console.WriteLine($"{fileCount}개의 파일\t{size} 바이트");
            Console.WriteLine($"{folderCount}개의 디렉터리\t{drive.TotalFreeSpace} 바이트 남음");
        }


    }
}
