using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Command.Data;

namespace Command.IOException
{
    class InputProcessor
    {
        private CursorPosition cursorPosition;
        private char userInputCharacter;

        public InputProcessor()
        {
            cursorPosition = CursorPosition.GetInstance();
        }

        /// <summary>
        /// cmd창에서 기능구현이 가능한 키들을 받고, 키마다 설저한 상수를 반환하는 메소드입니다.
        /// </summary>
        /// <returns>설정된 키 상수</returns>
        public int PressOrderKey()
        {
            ConsoleKeyInfo keyInfo;
            while (true)
            {
                keyInfo = Console.ReadKey();
                switch (keyInfo.Key)
                {
                    case ConsoleKey.Escape:
                        return Constant.ESC;
                    case ConsoleKey.Enter:
                        return Constant.ENTER;
                    case ConsoleKey.Tab:
                        return Constant.TAB;
                    case ConsoleKey.UpArrow:
                        return Constant.UP;
                    case ConsoleKey.DownArrow:
                        return Constant.DOWN;
                    case ConsoleKey.LeftArrow:
                        return Constant.LEFT;
                    case ConsoleKey.RightArrow:
                        return Constant.RIGHT;
                    case ConsoleKey.Backspace:
                        return Constant.BACK;
                    default:
                        userInputCharacter = keyInfo.KeyChar;
                        return Constant.CHARACTER;
                }
            }
        }

        /// <summary>
        /// 사용자로부터 명령어를 입력받는 메소드입니다.
        /// </summary>
        /// <returns>사용자가 입력한 명령어</returns>
        public string CommandFromUser()
        {
            string command = "";
            int cursorLeft, cursorTop;
            cursorPosition.SetCursor();

            while (true)
            {
                cursorLeft = Console.CursorLeft;
                cursorTop = Console.CursorTop;
                switch(PressOrderKey())
                {
                    case Constant.ESC:
                        command = "";
                        SetCursorPositionAndWrite(cursorPosition.CursorLeft, cursorPosition.CursorTop, command);
                        break;
                    case Constant.ENTER:
                        return command;
                    case Constant.TAB:      // 입력 무시
                        Console.SetCursorPosition(cursorLeft, cursorTop);
                        break;
                    case Constant.UP:       // 입력 무시
                        Console.SetCursorPosition(cursorLeft, cursorTop);
                        break;
                    case Constant.DOWN:     // 입력 무시
                        Console.SetCursorPosition(cursorLeft, cursorTop);
                        break;
                    case Constant.LEFT:
                        MoveLeft(cursorLeft, cursorTop, command);
                        break;
                    case Constant.RIGHT:
                        MoveRight(cursorLeft, cursorTop, command);
                        break;
                    case Constant.BACK:
                        command = RemoveOneLetter(cursorLeft, cursorTop, command);
                        break;
                    case Constant.CHARACTER:
                        command += userInputCharacter;
                        break;
                }
            }
        }

        /// <summary>
        /// 커서를 왼쪽으로 옮기는 메소드입니다.
        /// </summary>
        /// <param name="cursorLeft">이전 커서 위치</param>
        /// <param name="cursorTop">이전 커서 위치</param>
        /// <param name="command">명령어</param>
        public void MoveLeft(int cursorLeft, int cursorTop, string command)
        {
            SetCursorPositionAndWrite(cursorPosition.CursorLeft, cursorPosition.CursorTop, command);

            if (cursorPosition.CursorTop < cursorTop)
            {
                if (0 < cursorLeft)
                    Console.SetCursorPosition(cursorLeft - 1, cursorTop);
                else
                    Console.SetCursorPosition(Console.WindowWidth - 1, cursorTop - 1);
            }
            else
            {
                if (cursorPosition.CursorLeft < cursorLeft)
                    Console.SetCursorPosition(cursorLeft - 1, cursorTop);
                else
                    Console.SetCursorPosition(cursorLeft, cursorTop);
            }
        }

        /// <summary>
        /// 커서를 오른쪽으로 옮기는 메소드입니다.
        /// </summary>
        /// <param name="cursorLeft">이전 커서 위치</param>
        /// <param name="cursorTop">이전 커서 위치</param>
        /// <param name="command">명령어</param>
        public void MoveRight(int cursorLeft, int cursorTop, string command)
        {
            SetCursorPositionAndWrite(cursorPosition.CursorLeft, cursorPosition.CursorTop, command);

            if (cursorPosition.CursorTop < cursorTop)
            {
                if (cursorPosition.CursorLeft + command.Length - Console.WindowWidth > cursorLeft)
                    Console.SetCursorPosition(cursorLeft + 1, cursorTop);
                else
                    Console.SetCursorPosition(cursorLeft, cursorTop);
            }
            else
            {
                if (Console.WindowWidth - 1 > cursorLeft)
                    Console.SetCursorPosition(cursorLeft + 1, cursorTop);
                else
                    Console.SetCursorPosition(0, cursorTop + 1);
            }
        }

        /// <summary>
        /// command에서 커서 앞 글자를 지우는 메소드입니다.
        /// </summary>
        /// <param name="cursorLeft">이전 커서 위치</param>
        /// <param name="cursorTop">이전 커서 위치</param>
        /// <param name="command">명령어</param>
        /// <returns></returns>
        public string RemoveOneLetter(int cursorLeft, int cursorTop, string command)
        {
            if (command.Length == 0)
            {
                Console.SetCursorPosition(cursorLeft, cursorTop);
                return command;
            }

            if (cursorPosition.CursorTop < Console.CursorTop)
            {
                command = command.Remove(Console.WindowWidth - cursorPosition.CursorLeft + Console.CursorLeft, 1);
                SetCursorPositionAndWrite(0, Console.CursorTop, new string(' ', Console.WindowWidth));
                SetCursorPositionAndWrite(cursorPosition.CursorLeft, cursorPosition.CursorTop, command);
                Console.SetCursorPosition(cursorLeft - 1, Console.CursorTop);
            }
            else
            {
                command = command.Remove(Console.CursorLeft - cursorPosition.CursorLeft, 1);
                SetCursorPositionAndWrite(cursorPosition.CursorLeft, cursorPosition.CursorTop, new string(' ', Console.WindowWidth - Console.CursorLeft));
                SetCursorPositionAndWrite(cursorPosition.CursorLeft, cursorPosition.CursorTop, command);
                Console.SetCursorPosition(cursorLeft - 1, Console.CursorTop);
            }

            return command;
        }

        /// <summary>
        /// 지정된 위치에 커서를 맞추고 문자열을 출력하는 메소드입니다.
        /// </summary>
        /// <param name="cursorLeft">cursorLeft</param>
        /// <param name="cursorTop">cursorTop</param>
        /// <param name="comment">출력할 문자열</param>
        public void SetCursorPositionAndWrite(int cursorLeft, int cursorTop, string comment)
        {
            Console.SetCursorPosition(cursorLeft, cursorTop);
            Console.Write(comment);
        }
    }
}
