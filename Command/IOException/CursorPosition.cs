using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Command.IOException
{
    class CursorPosition
    {
        private int cursorLeft;
        private int cursorTop;
        private static CursorPosition cursorPosition;

        /// <summary>
        /// 싱글톤 패턴 사용을 위한 CursorPosition 객체 호출 메소드입니다.
        /// CursorPosition 객체를 반환합니다.
        /// </summary>
        /// <returns>CursorPosition 객체</returns>
        public static CursorPosition GetInstance()
        {
            if (cursorPosition == null) cursorPosition = new CursorPosition();
            
            return cursorPosition;
        }

        public void SetCursor()
        {
            cursorLeft = Console.CursorLeft;
            cursorTop = Console.CursorTop;
        }

        public void SetCursor(int cursorLeft, int cursorTop)
        {
            this.cursorLeft = cursorLeft;
            this.cursorTop = cursorTop;
        }

        public void GetCursor(out int cursorLeft, out int cursorTop)
        {
            cursorLeft = this.cursorLeft;
            cursorTop = this.cursorTop;
        }

        public int CursorLeft
        {
            get { return cursorLeft; }
            set { cursorLeft = value; }
        }

        public int CursorTop
        {
            get { return cursorTop; }
            set { cursorTop = value; }
        }
    }
}
