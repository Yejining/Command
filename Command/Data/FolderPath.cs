using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Command.Data
{
    class FolderPath
    {
        private string path;
        private static FolderPath folderPath;
        
        /// <summary>
        /// 싱글톤 패턴 사용을 위한 FolerPath 객체 호출 메소드입니다.
        /// path를 초기화하고 FolderPath 객체를 반환합니다.
        /// </summary>
        /// <returns>FolderPath 객체</returns>
        public static FolderPath GetInstance()
        {
            if (folderPath == null) folderPath = new FolderPath();

            folderPath.InitializePath();

            return folderPath;
        }

        /// <summary>
        /// path를 초기화하는 메소드입니다.
        /// Current User Directory를 찾아 path로 설정해줍니다.
        /// </summary>
        private void InitializePath()
        {
            path = Directory.GetParent(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)).FullName;

            if (Environment.OSVersion.Version.Major >= 6)
            {
                path = Directory.GetParent(path).ToString();
            }
        }

        /// <summary>
        /// count만큼 상위 폴더로 이동하는 메소드입니다.
        /// </summary>
        /// <param name="count">상위 폴더로 이동하는 횟수</param>
        public void SetUpperDirectory(int count)
        {
            DirectoryInfo directory;

            for (int counter = 0; counter < count; counter++)
            {
                directory = Directory.GetParent(path);
                if (directory != directory.Root)
                {
                    Directory.SetCurrentDirectory(directory.ToString());
                    path = Directory.GetCurrentDirectory();
                }
            }
        }

        /// <summary>
        /// 경로를 변경하는 메소드입니다.
        /// </summary>
        /// <param name="path">새로운 경로</param>
        public void SetCurrentDirectory(string path)
        {
            Directory.SetCurrentDirectory(path);
            this.path = Directory.GetCurrentDirectory();
        }

        public string PathToUse()
        {
            return path + ">";
        }

        public string Path
        {
            get { return path; }
            set { path = value; }
        }
    }
}
