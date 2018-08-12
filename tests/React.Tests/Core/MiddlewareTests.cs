#if NETCOREAPP2_0

using System.Runtime.InteropServices;
using Microsoft.AspNetCore.Hosting;
using Moq;
using React.AspNet;
using Xunit;

namespace React.Tests.Core
{
	public class MiddlewareTests
    {
		[Fact]
		public void ForwardSlashesAreTransformed()
		{
			var environment = new Mock<IHostingEnvironment>();
			if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
			{
				environment.Setup(x => x.WebRootPath).Returns("c:\\temp");
				Assert.Equal("c:\\temp\\wwwroot\\script.js", new AspNetFileSystem(environment.Object).MapPath("~/wwwroot/script.js"));
			}
			else
			{
				environment.Setup(x => x.WebRootPath).Returns("/var/www");
				Assert.Equal("/var/www/wwwroot/script.js", new AspNetFileSystem(environment.Object).MapPath("~/wwwroot/script.js"));
			}
		}
	}
}
#endif
