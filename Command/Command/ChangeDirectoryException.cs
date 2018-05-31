using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;

using Command.Data;

namespace Command.Command
{
    class ChangeDirectoryException
    {
        // 명령어 다음에 공백이 없는 경우
        public bool SpaceAbsense(string command, out string renewedCommand)
        {
            string word = command.Split(' ')[0];
            renewedCommand = command;

            switch (word[0])
            {
                case '\\':  // '\'으로 시작하는 경우
                    return true;
                case '.':   // '.'으로 시작하는 경우
                    return StartWithDot(renewedCommand, out renewedCommand);
                default:
                    break;
            }
            
            // ,;=으로 이루어진 경우
            if (Regex.IsMatch(word, Constant.NEGLIGIBLE))
            {
                renewedCommand = renewedCommand.Remove(0, word.Length);
                return true;
            }
            // .이 포함된 경우
            else if (Regex.IsMatch(word, "\\."))
            {
                Console.WriteLine("지정된 경로를 찾을 수 없습니다.\n");
                return false;
            }

            Console.WriteLine($"\'cd{command}\'은(는) 내부 또는 외부 명령, 실행할 수 있는 프로그램, 또는\n배치 파일이 아닙니다.\n");
            return false;
        }

        public bool StartWithDot(string command, out string renewedCommand)
        {
            renewedCommand = command;

            while (true)
            {
                // .이 없는 경우
                if (Regex.IsMatch(renewedCommand, "^[^\\.]"))
                    return true;
                // . 또는 .\인 경우 
                else if (Regex.IsMatch(renewedCommand, "^(\\.)\\\\*(\\s)*$"))
                    Console.WriteLine();
                // ..이거나 ..\인 경우
                else if (Regex.IsMatch(renewedCommand, "^(\\.\\.)"))
                {
                    renewedCommand = Regex.Replace(renewedCommand, "^(\\.\\.)$", "..\\");
                    return CheckPath(renewedCommand.Split(' ')[0]);
                }
                // .이 3개 이상이거나 .뒤에 다른 문자가 오는 경우
                else if (!Regex.IsMatch(renewedCommand, "[^\\.^\\s]"))
                    Console.WriteLine();
                // 기타
                else
                    Console.WriteLine("지정된 경로를 찾을 수 없습니다.\n");

                return false;
            }
        }

        public bool CheckPath(string command)
        {
            // 파일 이름, 디렉터리 이름 또는 볼륨 레이블 오류
            if (!IsCorrectDivision(command))
                return false;

            string path = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), command));

            // 드라이브 바꾸는지 검사
            if (!ConvertDrive(path))
                return false;
            
            if (Directory.Exists(path))
            {
                Directory.SetCurrentDirectory(path);
                Console.WriteLine();
            }
            else
            {
                Console.WriteLine("지정된 경로를 찾을 수 없습니다.\n");
            }

            return false;
        }

        public bool IsCorrectDivision(string path)
        {
            if (Regex.IsMatch(path, ":"))
            {
                string drive = Regex.Match(path, ".*(?=:)").ToString();
                return IsExistDrive(drive);
            }

            return true;
        }

        public bool IsExistDrive(string drive)
        {
            drive += ":\\";

            foreach (string localDrive in Directory.GetLogicalDrives())
                if (drive == localDrive)
                    return true;

            Console.WriteLine("파일 이름, 디렉터리 이름 또는 볼륨 레이블 구문이 잘못되었습니다.\n");
            return false;
        }

        public bool ConvertDrive(string path)
        {
            string currentDrive = Directory.GetDirectoryRoot(Directory.GetCurrentDirectory());
            string pathDrive = Directory.GetDirectoryRoot(path);

            // 현재 드라이브와 사용자가 입력한 드라이브가 다를 경우
            if (currentDrive != pathDrive)
            {
                foreach(string drive in Directory.GetLogicalDrives())
                {
                    if (drive == pathDrive)
                    {
                        Directory.SetCurrentDirectory(Directory.GetDirectoryRoot(path));
                        return true;
                    }
                }

                Console.WriteLine("시스템이 지정된 드라이브를 찾을 수 없습니다.\n");
                return false;
            }

            return true;
        }
    
        public string GetHomeDirectory()
        {
            string path;

            path = Directory.GetParent(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)).FullName;
            if (Environment.OSVersion.Version.Major >= 6)
                path = Directory.GetParent(path).ToString();
            return path;
        }
    }
}
