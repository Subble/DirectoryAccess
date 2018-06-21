using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace DirectoryAccess
{
    static class ExtensionsAndTools
    {
        public static DirectoryInfo CreateDirectoryIfNew(string path)
        {
            var dir = new DirectoryInfo(path);

            if (!dir.Exists) dir.Create();

            return dir;
        }
    }
}
