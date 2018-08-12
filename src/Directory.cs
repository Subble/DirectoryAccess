using Subble.Core.Config;
using Subble.Core.Storage;
using System;
using System.Linq;
using System.IO;
using System.Reflection;

using static DirectoryAccess.ExtensionsAndTools;

namespace DirectoryAccess
{
    public class Directory : IDirectory
    {
        protected const string NAME_BASE = "DATA";

        protected const string NAME_TEMPFOLDER = "Temp";
        protected const string NAME_PRIVATE = "Private";
        protected const string NAME_PUBLIC = "Public";

        protected const string KEY_TEMPFOLDER = "IDirectory.TempFolder.Path";
        protected const string KEY_PRIVATE = "IDirectory.PrivateDirectory.Path";
        protected const string KEY_PUBLIC = "IDirectory.PublicDirectory.Path";

        private readonly IConfigManager _configs;
        private readonly string _workingDir;

        public Directory(IConfigManager config, string workingDirectory)
        {
            _configs = config;
            _workingDir = workingDirectory;

            ResetDirectory(KEY_TEMPFOLDER, NAME_TEMPFOLDER);
            ResetDirectory(KEY_PRIVATE, NAME_PRIVATE);
            ResetDirectory(KEY_PUBLIC, NAME_PUBLIC);
        }

        public DirectoryInfo TempDirectory
            => LoadDirectoryPath(KEY_TEMPFOLDER, NAME_TEMPFOLDER);

        public DirectoryInfo PrivateDirectory
            => LoadDirectoryPath(KEY_PRIVATE, NAME_PRIVATE);

        public DirectoryInfo PublicDirectory
            => LoadDirectoryPath(KEY_PUBLIC, NAME_PUBLIC);

        public DirectoryInfo GetDirectory(string pluginGuid, bool isPublic = false)
        {
            DirectoryInfo rootDir = isPublic ? PublicDirectory : PrivateDirectory;

            if (string.IsNullOrEmpty(pluginGuid))
                throw new ArgumentNullException("pluginGuid", "param can't be null or empty");

            var path = Path.Combine(rootDir.FullName, pluginGuid);
            return CreateDirectoryIfNew(path);
        }

        public bool SetPrivateDirectory(DirectoryInfo directory)
            => SetDirectory(directory, KEY_PRIVATE);

        public bool SetPublicDirectory(DirectoryInfo directory)
            => SetDirectory(directory, KEY_PUBLIC);

        public bool SetTempDirectory(DirectoryInfo directory)
            => SetDirectory(directory, KEY_TEMPFOLDER);

        /// <summary>
        /// Get the root directory that will have by default the temp, private and public directory
        /// </summary>
        /// <param name="subDirectory"></param>
        /// <returns></returns>
        private DirectoryInfo GetRootDirectory(string subDirectory = "")
        {
            var pathsToCombine = new [] {
                _workingDir,
                NAME_BASE
            };

            if (!string.IsNullOrEmpty(subDirectory))
            {
                pathsToCombine = pathsToCombine
                    .Append(subDirectory)
                    .ToArray();
            }

            var path = Path.Combine(pathsToCombine);
            return CreateDirectoryIfNew(path);
        }

        /// <summary>
        /// Load directory path from config file
        /// if key-value doesn't exist, the default value is returned
        /// </summary>
        /// <param name="key"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        private DirectoryInfo LoadDirectoryPath(string key, string name)
        {
            DirectoryInfo dir = null;

            _configs
                .Get<string>(key)
                .Some(path =>
                {
                    dir = CreateDirectoryIfNew(path);
                })
                .None(() =>
                {
                    dir = GetRootDirectory(name);
                });

            return dir;
        }

        /// <summary>
        /// Reset the value of key-value for dir path
        /// </summary>
        /// <param name="key"></param>
        /// <param name="name"></param>
        private void ResetDirectory(string key, string name)
        {
            var dir = LoadDirectoryPath(key, name);
            SetDirectory(dir, key);
        }

        /// <summary>
        /// Set the value of the key-value path
        /// </summary>
        /// <param name="info"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        private bool SetDirectory(DirectoryInfo info, string key)
        {
            if (info is null || string.IsNullOrEmpty(key))
                return false;

            if (info.Exists)
            {
                _configs.Set(key, info.FullName);
                return true;
            }

            return false;
        }
    }
}
