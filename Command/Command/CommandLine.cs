using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using Command.Data;

namespace Command.Command
{
    class CommandLine
    {
        private string path;

        public void RunCommand()
        {
            Console.Write(Constant.START);

            path = Directory.GetParent(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)).FullName;

            if (Environment.OSVersion.Version.Major >= 6)
            {
                path = Directory.GetParent(path).ToString();
            }

            path = path.Insert(path.Length, ">");

            ReadCommand();
        }

        public void ReadCommand()
        {


            Console.Write(path);

        }
    }
}
