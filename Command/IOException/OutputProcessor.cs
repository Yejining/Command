using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
        /// help 명령어 실행시 도움말을 출력해주는 메소드입니다.
        /// </summary>
        public void PrintHelp()
        {
            foreach (string help in Constant.HELP)
            {
                Console.WriteLine(help);
            }
        }
    }
}
