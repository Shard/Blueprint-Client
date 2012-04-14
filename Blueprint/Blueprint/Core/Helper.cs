using System;
using System.IO;
using System.Security;
using System.Security.AccessControl;


namespace Blueprint
{
    class Helper
    {

        public static bool mkdir(string path)
        {
            System.IO.Directory.CreateDirectory(path);
            DirectoryInfo info = new DirectoryInfo(path);
            System.Security.Principal.WindowsIdentity self = System.Security.Principal.WindowsIdentity.GetCurrent();
            System.Security.AccessControl.DirectorySecurity ds = info.GetAccessControl();
            ds.AddAccessRule(new FileSystemAccessRule(self.Name, FileSystemRights.FullControl, InheritanceFlags.ObjectInherit | InheritanceFlags.ContainerInherit, PropagationFlags.None, AccessControlType.Allow));
            info.SetAccessControl(ds);
            return true;
        }

    }
}
