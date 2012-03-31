using System;
using System.Net;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Lidgren.Network;


namespace Blueprint
{
    class Client
    {

        public Config Config;
        public NetClient NetClient;
        public List<string> Messages;

        public Client(Config config)
        {
            Config = config;
            Messages = new List<string>();
            NetPeerConfiguration netconfig = new NetPeerConfiguration("client");
            NetClient = new NetClient(netconfig);
        }

        public void Initialize()
        {
            NetClient.Start();
            NetClient.Connect(Config.Join, 8877);

            NetOutgoingMessage msg = NetClient.CreateMessage();
            msg.Write("Yo, Whats up M.r White");
            NetClient.SendMessage(msg, NetDeliveryMethod.ReliableOrdered);
            Messages.Add("sent packet");
        }

        public void Update()
        {

        }

    }
}
