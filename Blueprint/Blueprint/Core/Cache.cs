using System;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Blueprint.Core
{
    class Cache
    {

        public string DataFolder;

        public void Initialize(Config config)
        {
            DataFolder = config.DataFolder;
        }

        public string read(string path)
        {
            if (File.Exists(DataFolder + "\\data\\" + path.Replace("/", "-")))
            {
                StreamReader reader = new StreamReader(DataFolder + "\\data\\" + path.Replace("/", "-"));
                return reader.ReadToEnd();
            }
            return null;
        }

        public Texture2D read(string path, GraphicsDevice graphics)
        {
            if (File.Exists(DataFolder + "\\data\\" + path.Replace("/", "-")))
            {
                StreamReader reader = new StreamReader(DataFolder + "\\data\\" + path.Replace("/", "-"));
                return Texture2D.FromStream(graphics, reader.BaseStream);
            }
            return null;
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
