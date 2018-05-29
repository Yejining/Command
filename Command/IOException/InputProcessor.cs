using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

using Command.Data;

namespace Command.IOException
{
    class InputProcessor
    {
        private CursorPosition cursorPosition;
        FolderPath folderPath;
        private char userInputCharacter;

        public InputProcessor()
        {
            cursorPosition = CursorPosition.GetInstance();
            folderPath = FolderPath.GetInstance();
        }

        /// <summary>
        /// 사용자로부터 명령어를 입력받고 리턴해주는 메소드입니다.
        /// </summary>
        /// <returns>명령어</returns>
        public string GetCommand()
        {
            Console.Write("\n" + folderPath.PathToUse());
            return CommandFromUser();
        }

        /// <summary>
        /// 사용자에게 질문을 출력해주고 답을 얻어 반환하는 메소드입니다.
        /// </summary>
        /// <param name="question"></param>
        /// <returns>사용자 답변</returns>
        public string GetAnswer(string question)
        {
            Console.Write(question);
            return CommandFromUser();
        }

        public string Order(string command)
        {
            if (command.Length == 0) return "";
            else if (Regex.IsMatch(command.ToLower(), Constant.VALID_CD)) return "CD";
            else if ((Regex.IsMatch(command.ToLower(), Constant.VALID_DIR))) return "DIR";
            else if ((Regex.IsMatch(command.ToLower(), Constant.VALID_COPY))) return "COPY";
            else if (Regex.IsMatch(command.ToLower(), Constant.VALID_MOVE)) return "MOVE";
            else if (Regex.IsMatch(command.ToLower(), Constant.VALID_EXIT)) return "EXIT";
            else if (Regex.IsMatch(command.ToLower(), Constant.VALID_CLS)) return "CLS";
            else return "default";
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
                        ClearCommandLine(cursorLeft, cursorTop, command);
                        command = "";
                        break;
                    case Constant.ENTER:
                        Console.SetCursorPosition(0, cursorTop + 1);
                        return command;
                    case Constant.TAB:      // 입력 무시
                        SetCursorPositionAndWrite(cursorPosition.CursorLeft, cursorPosition.CursorTop, command);
                        Console.SetCursorPosition(cursorLeft, cursorTop);
                        break;
                    case Constant.UP:       // 입력 무시
                        SetCursorPositionAndWrite(cursorPosition.CursorLeft, cursorPosition.CursorTop, command);
                        Console.SetCursorPosition(cursorLeft, cursorTop);
                        break;
                    case Constant.DOWN:     // 입력 무시
                        SetCursorPositionAndWrite(cursorPosition.CursorLeft, cursorPosition.CursorTop, command);
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
                        command = AddOneLetter(cursorLeft, cursorTop, command);
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
                MoveLeft(cursorLeft, cursorTop);
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
        /// 커서를 왼쪽으로 옮기는 메소드입니다.
        /// 명령문의 시작줄과 현재 커서가 위치한 줄이 다를때 적용시킵니다.
        /// </summary>
        /// <param name="cursorLeft">커서 위치</param>
        /// <param name="cursorTop">커서 위치</param>
        public void MoveLeft(int cursorLeft, int cursorTop)
        {
            if (0 < cursorLeft)
                Console.SetCursorPosition(cursorLeft - 1, cursorTop);
            else
                Console.SetCursorPosition(Console.WindowWidth - 1, cursorTop - 1);
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
                if (IndexOfCommand(cursorLeft, cursorTop, command) < command.Length)
                    MoveRight(cursorLeft, cursorTop);
                else
                    Console.SetCursorPosition(cursorLeft, cursorTop);
            }
            else
            {
                if (cursorLeft - cursorPosition.CursorLeft < command.Length)
                    MoveRight(cursorLeft, cursorTop);
                else
                    Console.SetCursorPosition(cursorLeft, cursorTop);
            }
        }

        /// <summary>
        /// 커서를 오른쪽으로 옮기는 메소드입니다.
        /// </summary>
        /// <param name="cursorLeft">이전 커서 위치</param>
        /// <param name="cursorTop">이전 커서 위치</param>
        public void MoveRight(int cursorLeft, int cursorTop)
        {
            if (cursorLeft < Console.WindowWidth - 1)
                Console.SetCursorPosition(cursorLeft + 1, cursorTop);
            else
                Console.SetCursorPosition(0, cursorTop + 1);
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
            // 커서가 문자열 앞에 있는 경우
            if (cursorPosition.CursorTop == cursorTop && cursorPosition.CursorLeft == cursorLeft)
            {
                Console.SetCursorPosition(cursorPosition.CursorLeft, cursorPosition.CursorTop);
                return command;
            }

            if (cursorPosition.CursorTop < cursorTop)
            {
                command = command.Remove(IndexOfCommand(cursorLeft, cursorTop, command) - 1, 1);
            }
            else if (cursorPosition.CursorLeft != cursorLeft)
            {
                command = command.Remove(cursorLeft - cursorPosition.CursorLeft - 1, 1);
            }

            SetCursorPositionAndWrite(cursorPosition.CursorLeft, cursorPosition.CursorTop, new string(' ', command.Length + 1));
            SetCursorPositionAndWrite(cursorPosition.CursorLeft, cursorPosition.CursorTop, command);
            MoveLeft(cursorLeft, cursorTop);

            return command;
        }

        /// <summary>
        /// 입력받은 문자가 유효한 값일 경우 명령문에 해당 문자를 추가하는 메소드입니다.
        /// </summary>
        /// <param name="cursorLeft">커서 위치</param>
        /// <param name="cursorTop">커서 위치</param>
        /// <param name="command">이전에 입력받은 명령어</param>
        /// <returns>갱신된 명령어</returns>
        public string AddOneLetter(int cursorLeft, int cursorTop, string command)
        {
            if (Regex.IsMatch(userInputCharacter.ToString(), Constant.VALID_LETTER))
            {
                command = command.Insert(IndexOfCommand(cursorLeft, cursorTop, command), userInputCharacter.ToString());
                cursorLeft++;
            }

            SetCursorPositionAndWrite(cursorPosition.CursorLeft, cursorPosition.CursorTop, command);
            Console.SetCursorPosition(cursorLeft, cursorTop);

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

        /// <summary>
        /// 입력된 명령어를 지우고 커서를 초기화하는 메소드입니다.
        /// </summary>
        /// <param name="cursorLeft">커서 위치</param>
        /// <param name="cursorTop">커서 위치</param>
        /// <param name="command">명령문</param>
        public void ClearCommandLine(int cursorLeft, int cursorTop, string command)
        {
            SetCursorPositionAndWrite(cursorPosition.CursorLeft, cursorPosition.CursorTop, new string(' ', command.Length + 1));
            Console.SetCursorPosition(cursorPosition.CursorLeft, cursorPosition.CursorTop);
        }

        /// <summary>
        /// 커서가 위치하고 있는 명령문의 인덱스를 반환하는 메소드입니다.
        /// </summary>
        /// <param name="cursorLeft">커서 위치</param>
        /// <param name="cursorTop">커서 위치</param>
        /// <param name="command">명령문</param>
        /// <returns>커서가 위치한 명령문의 인덱스</returns>
        public int IndexOfCommand(int cursorLeft, int cursorTop, string command)
        {
            return Console.WindowWidth - cursorPosition.CursorLeft + ((cursorTop - cursorPosition.CursorTop - 1) * Console.WindowWidth) + cursorLeft;
        }
    }
}
