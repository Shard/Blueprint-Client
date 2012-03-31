﻿using System;
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

        public Config(string[] args)
        {
            if(args.Length < 1){
                args = new string[1];
                args[0] = "blueprint://map:17/host:true"; // Default
            }

            // Default Arguments
            MapId = 15;
            Hosting = false;
            Join = null;
            UserId = 1;
            Argument = "No Arguments";
            IP = this.getIP();

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

        public void SetupRegistery()
        {
            // Registery
            string gameLocation = System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase;
            string fullLocation = System.IO.Path.GetDirectoryName(gameLocation);
            fullLocation = "\"" + fullLocation.Replace("file:\\", "") + "\\blueprint.exe\" \"%1\"";

            RegistryKey reg = Registry.ClassesRoot.OpenSubKey("blueprint");
            if (reg == null)
            {
                // Need to initialize Registery
                RegistryKey blueprint = Registry.ClassesRoot.CreateSubKey("blueprint");
                Registry.ClassesRoot.OpenSubKey("blueprint", true)
                    .SetValue("", "URL: Blueprint Protocol", RegistryValueKind.String);
                Registry.ClassesRoot.OpenSubKey("blueprint", true)
                    .SetValue("URL Protocol", "");

                Registry.ClassesRoot.CreateSubKey("blueprint\\shell");
                Registry.ClassesRoot.CreateSubKey("blueprint\\shell\\open");
                RegistryKey commandKey = Registry.ClassesRoot.CreateSubKey("blueprint\\shell\\open\\command");

                commandKey.SetValue("", fullLocation, RegistryValueKind.String);
            }
            else
            {
                //RegistryKey commandKey = Registry.ClassesRoot.CreateSubKey("blueprint\\shell\\open\\command");
                //commandKey.SetValue("", fullLocation, RegistryValueKind.String);
                // Make sure the path is ok, Send the server the update command if it needs updating
            }
        }

    }
}