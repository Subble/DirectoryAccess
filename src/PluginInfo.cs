using Subble.Core.Plugin;
using System;

namespace DirectoryAccess
{
    public class PluginInfo : IPluginInfo
    {
        public string GUID => "f033871b-d05d-42c5-a35d-7700d4f5952b";

        public string Name => "DirectoryAccess";

        public string Creator => "David Pires";

        public string Repository => "https://github.com/Subble/DirectoryAccess";

        public string Support => "https://github.com/Subble/DirectoryAccess/issues";

        public string Licence => "MIT";
    }
}
