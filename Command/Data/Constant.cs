using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Command.Data
{
    class Constant
    {
        public const string START = "Microsoft Windows [Version 10.0.16299.431]\n(c) 2017 Microsoft Corporation. All rights reserved.\n\n";

        public const string VALID_LETTER = "[0-9a-zA-Zㄱ-ㅎㅏ-ㅣ가-힣[`~!@#$%^&*()\\-_=+\\{\\}\\[\\]\\\\\\|:;\"\'<>,.?/ ]";

        public const int ESC = 0;
        public const int ENTER = 1;
        public const int TAB = 2;
        public const int UP = 3;
        public const int DOWN = 4;
        public const int LEFT = 5;
        public const int RIGHT = 6;
        public const int BACK = 7;
        public const int CHARACTER = 8;
    }
}
