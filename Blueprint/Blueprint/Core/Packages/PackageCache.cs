using System;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Blueprint
{
    class PackageCache
    {

        public string DataFolder;

        public PackageCache(Config config)
        {
            DataFolder = config.DataFolder;
        }

        public Texture2D Read(string path, GraphicsDevice graphics)
        {
            if (File.Exists(DataFolder + "\\data\\" + path.Replace("/", "-")))
            {
                StreamReader reader = new StreamReader(DataFolder + "\\data\\" + path.Replace("/", "-"));
                Texture2D texture = Texture2D.FromStream(graphics, reader.BaseStream);
                reader.Close();
                return texture;
            }
            return null;
        }
        public string Read(string path)
        {
            if (File.Exists(DataFolder + "\\data\\" + path.Replace("/", "-")))
            {
                StreamReader reader = new StreamReader(DataFolder + "\\data\\" + path.Replace("/", "-"));
                return reader.ReadToEnd();
            }
            return null;
        }

        public bool Write(string path, Texture2D texture)
        {
            if (!File.Exists(DataFolder + "\\data"))
            {
                Helper.mkdir(DataFolder + "\\data");
            }

            Stream stream = new FileStream(DataFolder + "\\data\\" + path.Replace("/", "-"), FileMode.OpenOrCreate);
            texture.SaveAsPng(stream, texture.Bounds.Width, texture.Bounds.Height);

            return true;
        }
        public bool Write(string path, string data)
        {
            if (!File.Exists(DataFolder + "\\data")) { Helper.mkdir(DataFolder + "\\data"); }

            StreamWriter stream = new StreamWriter(DataFolder + "\\data\\" + path.Replace("/", "-"));
            stream.Write(data);
            stream.Close();

            return true;
        }

    }
}
