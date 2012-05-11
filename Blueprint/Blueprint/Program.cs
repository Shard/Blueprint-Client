using System;
using System.IO;
using System.Diagnostics;
using System.Windows.Forms;
using System.Security.AccessControl;

namespace Blueprint
{
#if WINDOWS || XBOX
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {

            if (!Program.setupReg()) { return; }

            using (BlueprintGame game = new BlueprintGame(args))
            {
                game.Run();
            }
        }

        static bool setupReg()
        {

            string path = System.IO.Path.GetFullPath(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\blueprint");
            if (!File.Exists(path))
            {
                System.IO.Directory.CreateDirectory(path);
                DirectoryInfo info = new DirectoryInfo(path);
                System.Security.Principal.WindowsIdentity self = System.Security.Principal.WindowsIdentity.GetCurrent();
                System.Security.AccessControl.DirectorySecurity ds = info.GetAccessControl();
                ds.AddAccessRule(new FileSystemAccessRule(self.Name, FileSystemRights.FullControl, InheritanceFlags.ObjectInherit | InheritanceFlags.ContainerInherit, PropagationFlags.None, AccessControlType.Allow));
                info.SetAccessControl(ds);
            }

            FileStream stream = new FileStream(path + "\\path.txt", FileMode.OpenOrCreate);
            StreamReader reader = new StreamReader(stream);
            string currentPath = reader.ReadToEnd();
            reader.Close();
            stream.Close();

            string programLocation = System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase;
            string fullLocation = System.IO.Path.GetDirectoryName(programLocation);
            fullLocation = "\"" + fullLocation.Replace("file:\\", "") + "\\blueprint.exe\" \"%1\"";

            if (currentPath != fullLocation)
            {
                try
                {
                    Process.Start("BlueprintSetup.exe");
                }
                catch (Exception e)
                {
                    MessageBox.Show("Error running setup: " + e.Message);
                    return false;
                }
                StreamWriter newstream = new StreamWriter(path + "\\path.txt");
                newstream.Write(fullLocation);
                newstream.Close();
                
            }
            return true;
        }
    }
#endif
}

