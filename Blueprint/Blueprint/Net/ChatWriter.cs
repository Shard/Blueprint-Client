using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Blueprint
{
    class ChatWriter
    {

        public StringBuilder Message;
        public int Cursor;

        public ChatWriter()
        {
            Clear();
        }

        public void Clear()
        {
            Cursor = 0;
            Message = new StringBuilder();
        }

        public void Type(string message)
        {
            Message.Insert(Cursor, message);
            Cursor += message.Length;
        }

        public void Backspace()
        {
            if(Cursor == 0){ return; }
            Message.Remove(Cursor -1, 1);
            Cursor -= 1;
        }

        public void Delete()
        {
            if (Cursor >= Message.Length) { return; }
            Message.Remove(Cursor, 1);
        }

        public void MoveCursor(int amount)
        {
            if (Cursor + amount < 0) {
                Cursor = 0; 
            } else if (Cursor + amount > Message.Length) 
            { 
                Cursor = Message.Length;
            } else { 
                Cursor += amount;
            }
        }

    }
}
