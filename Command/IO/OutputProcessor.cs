using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Command.Data;

namespace Command.IO
{
    class OutputProcessor
    {
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
