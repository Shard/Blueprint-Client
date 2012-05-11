using System;
using System.Collections.Generic;
using System.Net;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Lidgren.Network;


namespace Blueprint
{
    class Client : NetBase
    {

        public Config Config;
        public NetClient NetClient;
        public List<string> Messages;

        // Other
        Texture2D PlayerTexture;
        Texture2D BarsTexture;

        public Client(Config config)
        {
            Config = config;
            Messages = new List<string>();
            NetPeerConfiguration netconfig = new NetPeerConfiguration("blueprint");
            netconfig.EnableMessageType(NetIncomingMessageType.DiscoveryResponse);
            NetClient = new NetClient(netconfig);
            NetClient.Start();
            Players = new List<Player>();
        }

        public void Initialize(Texture2D playerTexture, Texture2D barsTexture)
        {
            NetClient.DiscoverLocalPeers(8877);
            PlayerTexture = playerTexture;
            BarsTexture = barsTexture;
            //NetClient.Connect(Config.Join, 8877);

           // NetOutgoingMessage msg = NetClient.CreateMessage();
           // msg.Write("Yo, Whats up M.r White");
           // NetClient.SendMessage(msg, NetDeliveryMethod.ReliableOrdered);
           // Messages.Add("sent packet");
        }

        public void Update(Control control, Map map, Player player, GameTime time, Package package)
        {

            UpdatePlayers(control, map);

            NetOutgoingMessage msgout = NetClient.CreateMessage();
            msgout.Write(BuildCommand(player));
            NetClient.SendMessage(msgout, NetDeliveryMethod.Unreliable);


            NetIncomingMessage msg;
            while ((msg = NetClient.ReadMessage()) != null)
            {
                switch(msg.MessageType)
                {
                    case NetIncomingMessageType.DiscoveryResponse:
                        NetClient.Connect(msg.SenderEndpoint);

                        Player newplayer = new Player();
                        newplayer.Initialize(PlayerTexture, BarsTexture, package, map.Spawn);
                        Messages.Add("new player");
                        Players.Add(newplayer);
                        break;
                    case NetIncomingMessageType.Data:

                        HandleData(msg);
                        

                        break;
                    default:
                       // Messages.Add(msg.ReadString());
                        break;
                }
            }

        }

    }
}
