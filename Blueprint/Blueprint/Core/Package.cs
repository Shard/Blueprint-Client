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

        public string Server;
        public string DataFolder;
        public PackageCache Cache;

        public Package(Config config)
        {
            
            Server = config.Server;
            DataFolder = config.DataFolder;
            Cache = new PackageCache(config);

        }

        public string RemoteString(string path)
        {

            string cache = Cache.Read(path);
            if (cache != null) { return cache; }

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Server + path);
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            StreamReader reader = new StreamReader(response.GetResponseStream());
            string data = reader.ReadToEnd();
            reader.Close();
            Cache.Write(path, data);
            return data;
        }

        public Texture2D RemoteTexture(string path, GraphicsDevice graphics)
        {

            Texture2D texture = Cache.Read(path, graphics);
            if (texture != null) { return texture; }

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Server + path);
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

            Cache.Write(path, mapTexture);

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
