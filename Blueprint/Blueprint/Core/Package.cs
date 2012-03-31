using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Blueprint
{
    class Package
    {

        public Texture2D RemoteTexture(string path, GraphicsDevice graphics)
        {

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://local.blueprintgame.com:8888/" + path);
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            byte[] b = null;
            using (Stream stream = response.GetResponseStream())
            using (MemoryStream ms = new MemoryStream())
            {
                int count = 0;
                do
                {
                    byte[] buf = new byte[1024];
                    count = stream.Read(buf, 0, 1024);
                    ms.Write(buf, 0, count);
                } while (stream.CanRead && count > 0);
                b = ms.ToArray();
            }

            Texture2D mapTexture = Texture2D.FromStream(graphics, new MemoryStream(b));

            return mapTexture;

        }

        public Texture2D LocalTexture(string path, GraphicsDevice graphics)
        {
            StreamReader blocksReader = new StreamReader("C:\\Users\\Mark\\Desktop\\blocks.png");
            Stream blocks = blocksReader.BaseStream;
            Texture2D blockTexture = Texture2D.FromStream(graphics, blocks);
            return blockTexture;
        }

    }
}
