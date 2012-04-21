using System;
using System.Collections.Generic;
using System.Net;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Lidgren.Network;

namespace Blueprint
{

    /// <summary>
    /// Provides a base set of functionality for the Server and Client classes
    /// </summary>
    class NetBase
    {

        // Data
        public List<Player> Players;
        string Command;

        public void HandleData(NetIncomingMessage msg)
        {
            string data = msg.ReadString();

            NetCommand[] commands = NetParser.Parse(data);

            foreach (var command in commands)
            {
                switch (command.Type)
                {
                    case 'a': // Add a new player into the mix
                        break;
                    case 'd': // deletes a player from the mix
                        break;
                    case 'i': // Intention update
                        Players[command.PlayerIndex].Movement.Intention = new Intention(data.Substring(2, 4));
                        break;
                    case 'r': // Refresh Player
                        string[] bits = command.Value.Split(',');
                        Players[command.PlayerIndex].Movement.Area.X = Int32.Parse(bits[0]);
                        Players[command.PlayerIndex].Movement.Area.Y = Int32.Parse(bits[1]);
                        break;
                    case 'c': // Chat Message
                        break;
                    case 'p': // Pushback
                        break;
                }
            }
        }

        public void UpdatePlayers(Control control, Map map)
        {
            foreach (var player in Players)
            {
                player.Update(control, map);

                // Update Player
            }
        }

        public void Draw(SpriteBatch spriteBatch, Camera camera)
        {
            foreach (var player in Players)
            {
                player.Draw(spriteBatch, camera);
            }
        }


        public string BuildCommand(Player player)
        {

            string command = "i:" + player.Movement.Intention.ToString() + ";";
            
            // Should be run every X
            command += player.NetCommand();

            return command;

        }
    }
}
