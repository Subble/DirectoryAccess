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
        protected const string NAME_TEMPFOLDER = "Temp";
        protected const string NAME_LOCALDIR = "Local";
        protected const string NAME_SYNCDIR = "Shared";

        protected const string KEY_TEMPFOLDER = "IDirectory.TempFolder.Path";
        protected const string KEY_LOCALDIR = "IDirectory.LocalDirectory.Path";
        protected const string KEY_SYNCDIR = "IDirectory.SyncDirectory.Path";

        private readonly IConfigManager _configs;

        public Directory(IConfigManager config)
        {
            _configs = config;

            ResetDirectory(KEY_TEMPFOLDER, NAME_TEMPFOLDER);
            ResetDirectory(KEY_LOCALDIR, NAME_LOCALDIR);
            ResetDirectory(KEY_SYNCDIR, NAME_SYNCDIR);

            TempFolder = LoadDirectoryPath(KEY_TEMPFOLDER, NAME_TEMPFOLDER);
            LocalDirectory = LoadDirectoryPath(KEY_LOCALDIR, NAME_LOCALDIR);
            SyncDiretory = LoadDirectoryPath(KEY_SYNCDIR, NAME_SYNCDIR);
        }

        public DirectoryInfo TempFolder { get; private set; }

        public DirectoryInfo LocalDirectory { get; private set; }

        public DirectoryInfo SyncDiretory { get; private set; }

        public DirectoryInfo GetDirectory(string pluginGuid, bool sync = false)
        {
            DirectoryInfo rootDir = sync ? SyncDiretory : LocalDirectory;

            if (string.IsNullOrEmpty(pluginGuid))
                throw new ArgumentNullException("pluginGuid", "param can't be null or empty");

            var path = Path.Combine(rootDir.FullName, pluginGuid);
            var dir = CreateDirectoryIfNew(path);

            return dir;
        }

        public bool SetLocalDirectory(DirectoryInfo directory)
            => SetDirectory(directory, KEY_LOCALDIR);

        public bool SetSyncDirectory(DirectoryInfo directory)
            => SetDirectory(directory, KEY_SYNCDIR);

        public bool SetTempFolder(DirectoryInfo directory)
            => SetDirectory(directory, KEY_TEMPFOLDER);

        /// <summary>
        /// Get directory of Subble.exe
        /// </summary>
        /// <returns></returns>
        private DirectoryInfo GetRunningDirectory()
        {
            var dir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            return new DirectoryInfo(dir);
        }

        /// <summary>
        /// Get the root directory that will have by default the temp, local and shared directory
        /// </summary>
        /// <param name="subDirectory"></param>
        /// <returns></returns>
        private DirectoryInfo GetRootDirectory(string subDirectory = "")
        {
            var pathsToCombine = new [] {
                GetRunningDirectory().FullName,
                "Storage"
            };

            if (!string.IsNullOrEmpty(subDirectory))
            {
                pathsToCombine = pathsToCombine
                    .Append(subDirectory)
                    .ToArray();
            }

            var path = Path.Combine(pathsToCombine);
            var dir = CreateDirectoryIfNew(path);

            return dir;
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
