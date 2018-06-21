#if DEBUG
using Xunit;
using DirectoryAccess;
using Subble.Core.Config;
using Moq;
using System.Collections.Generic;

using static Subble.Core.Func.Option;
using System.IO;

namespace DirectoryAccess.Tests
{
    public class DirectoryTests
    {
        private IConfigManager GetConfigMock()
        {
            var dic = new Dictionary<string, string>();

            var mock = new Mock<IConfigManager>();

            mock.Setup(f => f.Get<string>(It.IsAny<string>()))
                .Returns((string s) => {

                    if(dic.ContainsKey(s))
                        return Some(dic[s]);

                    return None<string>();
                });

            mock.Setup(f => f.Set(It.IsAny<string>(), It.IsAny<string>()))
                .Callback((string k, string v) => dic[k] = v);

            return mock.Object;
        }

        [Fact]
        public void Initialization_Tests()
        {
            var dirAccess = new Directory(GetConfigMock());

            Assert.NotNull(dirAccess);

            Assert.True(dirAccess.TempFolder.Exists);
            Assert.True(dirAccess.LocalDirectory.Exists);
            Assert.True(dirAccess.SyncDiretory.Exists);
        }

        [Theory]
        [InlineData(@"z:\invaliddir")]
        [InlineData(@"dsadefsefse")]
        [InlineData(null)]
        public void Use_Invalid_Dirs(string dirPath)
        {
            var dirInfo = new DirectoryInfo(dirPath);
            var dirAccess = new Directory(GetConfigMock());

            Assert.NotNull(dirAccess);

            Assert.False(dirAccess.SetLocalDirectory(dirInfo));
            Assert.False(dirAccess.SetSyncDirectory(dirInfo));
            Assert.False(dirAccess.SetTempFolder(dirInfo));
        }
    }
}
#endif