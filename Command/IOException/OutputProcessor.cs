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

        public OutputProcessor()
        {
            folderPath = FolderPath.GetInstance();
        }

        public string GetCommand()
        {
            Console.Write(folderPath.PathToUse());
            return inputProcessor.CommandFromUser();
        }
    }
}
