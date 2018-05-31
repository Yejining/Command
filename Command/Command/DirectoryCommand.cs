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

        public void PrintDriveInformation()
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
        
        public void PrintDirectory(string path)
        {
            Console.WriteLine("출력\n");
        }
    }
}
