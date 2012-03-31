using System;
using System.Collections.Generic;
using System.Net;
using Microsoft.Xna.Framework;
using Lidgren.Network;

namespace Blueprint
{
    class Server
    {

        int Port;
        Config Config;
        NetServer NetServer;
        public List<ChatMessage> Messages;

        public Server(Config config)
        {

            Port = 8877;
            Config = config;
            Messages = new List<ChatMessage>();
        }

        public void Initialize()
        {
            
            // Tell server that server is avaliable
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://local.blueprintgame.com:8888/games/register/" + Config.MapId + "/" + Config.IP);
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            // Setup server socket
            NetPeerConfiguration config = new NetPeerConfiguration("host");
            config.Port = Port;
            NetServer = new NetServer(config);
            NetServer.Start();

        }

        public void Update(GameTime time)
        {
            NetIncomingMessage msg;
            while ((msg = NetServer.ReadMessage()) != null)
            {
                switch (msg.MessageType)
                {
                    case NetIncomingMessageType.VerboseDebugMessage:
                    case NetIncomingMessageType.DebugMessage:
                    case NetIncomingMessageType.WarningMessage:
                    case NetIncomingMessageType.ErrorMessage:
                        Messages.Add(new ChatMessage(msg.ReadString(), time));
                        break;
                    default:
                        Messages.Add(new ChatMessage("Unhandled request ("+msg.MessageType+"): " + msg.ReadString(), time));
                        break;
                }
                NetServer.Recycle(msg);
            }
        }

    }
}
