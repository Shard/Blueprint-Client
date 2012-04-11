using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Blueprint
{

    /// <summary>
    /// Provides functions to parse and create net commands which are used for client to client communication
    /// 
    /// Command Types:
    /// p - Pushback
    /// i - Intention
    /// r - Refresh
    /// c - Chat Message
    /// o - Owner (Is used like a namespace to point out that every command after it until another o command belongs to said player)
    /// </summary>
    class NetParser
    {

        public static NetCommand[] Parse(string data)
        {

            int cursor = 0;
            int cursor_end = 0;
            int command_count = 0;
            int current_player = 0;

            foreach (char c in data)
            {
                if(c == ':'){ command_count++; }
            }

            NetCommand[] commands = new NetCommand[command_count];

            for (int i = 0; i < command_count; i++)
            {
                cursor_end = data.IndexOf(';', cursor) - cursor;

                if (data[cursor] == 'p')
                {
                    current_player = Int16.Parse(data.Substring(cursor + 2, cursor_end));
                }
                else
                {
                    commands[i] = new NetCommand(data.Substring(cursor, cursor_end), current_player);
                }

                cursor = cursor_end + 1;
            }

            return commands;

        }
        
        

    }

    class NetCommand
    {

        public NetCommand(string data, int current_player)
        {
            String[] bits = data.Split(':');
            Type = bits[0][0];
            Value = bits[1];
            PlayerIndex = current_player;
        }

        public int PlayerIndex;
        public char Type;
        public string Value;

    }
}
