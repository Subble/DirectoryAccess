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

            Assert.True(dirAccess.TempDirectory.Exists);
            Assert.True(dirAccess.PrivateDirectory.Exists);
            Assert.True(dirAccess.PublicDirectory.Exists);
        }

        [Theory]
        [InlineData(@"z:\invaliddir")]
        [InlineData(@"dsadefsefse")]
        public void Ignore_Invalid_Dirs(string dirPath)
        {
            var dirInfo = new DirectoryInfo(dirPath);
            var dirAccess = new Directory(GetConfigMock());

            Assert.NotNull(dirAccess);

            Assert.False(dirAccess.SetPrivateDirectory(dirInfo));
            Assert.False(dirAccess.SetPublicDirectory(dirInfo));
            Assert.False(dirAccess.SetTempDirectory(dirInfo));
        }

        [Fact]
        public void Ignore_null_input()
        {
            var dirAccess = new Directory(GetConfigMock());

            Assert.NotNull(dirAccess);

            Assert.False(dirAccess.SetPrivateDirectory(null));
            Assert.False(dirAccess.SetPublicDirectory(null));
            Assert.False(dirAccess.SetTempDirectory(null));
        }
    }
}
#endif