using System;
using System.Collections.Generic;
using System.Net;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Lidgren.Network;

namespace Blueprint
{
    class Server : NetBase
    {

        int Port;
        Config Config;
        NetServer NetServer;
        public List<ChatMessage> Messages;
        GameTime Time;

        // Other
        Texture2D PlayerTexture;


        public Server(Config config)
        {

            Port = 8877;
            Config = config;
            Messages = new List<ChatMessage>();
            Players = new List<Player>();
        }

        public void Initialize(Texture2D playerTexture)
        {

            PlayerTexture = playerTexture;

            // Tell server that server is avaliable
            //HttpWebRequest request = (HttpWebRequest)WebRequest.Create( Config.Server + "/games/register/" + Config.MapId + "/" + Config.IP);
            //HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            // Setup server socket
            NetPeerConfiguration config = new NetPeerConfiguration("blueprint");
            config.EnableMessageType(NetIncomingMessageType.DiscoveryRequest);
            config.Port = Port;
            NetServer = new NetServer(config);
            NetServer.Start();

        }

        public void Message( string message )
        {
            Messages.Add(new ChatMessage(message, Time));
        }

        public void Update(Control control, Map map, Player player, GameTime time)
        {

            UpdatePlayers(control, map);

            NetOutgoingMessage msgout = NetServer.CreateMessage();
            msgout.Write(BuildCommand(player));
            NetServer.SendToAll(msgout, NetDeliveryMethod.Unreliable);

            Time = time;
            NetIncomingMessage msg;
            while ((msg = NetServer.ReadMessage()) != null)
            {
                switch (msg.MessageType)
                {
                    case NetIncomingMessageType.DiscoveryRequest:
                        NetServer.SendDiscoveryResponse(null, msg.SenderEndpoint);
                        break;
                    case NetIncomingMessageType.StatusChanged:
                        NetConnectionStatus status = (NetConnectionStatus)msg.ReadByte();
                        if (status == NetConnectionStatus.Connected)
                        {
                            Player newplayer = new Player(new Vector2(100, -100));
                            newplayer.Initialize(PlayerTexture);

                            Players.Add(newplayer);

                            Message(NetUtility.ToHexString(msg.SenderConnection.RemoteUniqueIdentifier) + " connected!");
                        }
                        else if(status == NetConnectionStatus.Disconnected)
                        {
                            Message(NetUtility.ToHexString(msg.SenderConnection.RemoteUniqueIdentifier) + " disconnected!");
                        }
                        break;
                    case NetIncomingMessageType.VerboseDebugMessage:
                    case NetIncomingMessageType.DebugMessage:
                    case NetIncomingMessageType.WarningMessage:
                    case NetIncomingMessageType.ErrorMessage:
                        Messages.Add(new ChatMessage(msg.ReadString(), time));
                        break;
                    case NetIncomingMessageType.Data:
                        HandleData(msg);
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
