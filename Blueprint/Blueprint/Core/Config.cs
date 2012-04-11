using System;
using Microsoft.Xna.Framework;
using Microsoft.Win32;
using System.Net;

namespace Blueprint
{
    class Config
    {

        public int MapId; // The id of the map
        public bool Hosting; // If true, the game is hosting or at least trying
        public string Join; // If joining a game, this will be a string for that address
        public int UserId; // The User id of the playing user
        public string Argument; // The argument that was passed in
        public string IP;
        public string Server; // Server that the client should be connecting to
        public string DataFolder;

        public Config(string[] args)
        {
            if(args.Length < 1){
                args = new string[1];
                args[0] = "blueprint://map:17/host:true/local:false"; // Default
            }

            // Default Arguments
            MapId = 15;
            Hosting = false;
            Join = null;
            UserId = 1;
            Argument = "No Arguments";
            IP = this.getIP();
            Server = "http://blueprintgame.com/";
            DataFolder = System.IO.Path.GetFullPath(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\blueprint");
            if (args.Length < 1) { return; }


            Argument = args[0].Replace("blueprint://", "");
            string[] options = Argument.Split('/');

            foreach(string option in options)
            {
                if (option == null) { continue; }
                string[] bits = option.Split(':');
                switch (bits[0])
                {
                    case "map":
                        MapId = Int32.Parse(bits[1]);
                        break;
                    case "host":
                        Hosting = Boolean.Parse(bits[1]);
                        break;
                    case "join":
                        Join = bits[1];
                        break;
                    case "user":
                        UserId = Int32.Parse(bits[1]);
                        break;
                    case "local":
                        if(Boolean.Parse(bits[1])){ Server = "http://local.blueprintgame.com:8888/"; }
                        break;

                }
            }
        }

        public string getIP()
        {
            
            IPHostEntry host;
            string localIP = "?";
            host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (IPAddress ip in host.AddressList)
            {
                if (ip.AddressFamily.ToString() == "InterNetwork")
                {
                    localIP = ip.ToString();
                }
            }
            return localIP;
        }

    }
}
