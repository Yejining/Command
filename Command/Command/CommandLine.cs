using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Command.Data;

namespace Command.Command
{
    class CommandLine
    {
        FolderPath folderPath;

        /// <summary>
        /// 프로그램이 실행될 때 호출되는 메소입니다.
        /// 파일 경로를 초기화하고 사용자로부터 명령문을 입력받습니다.
        /// </summary>
        public void RunCommand()
        {
            Console.Write(Constant.START);
            folderPath = FolderPath.GetInstance();
            ReadCommand();
        }

        public void ReadCommand()
        {
            Console.Write(folderPath.PathToUse());

        }
    }
}
