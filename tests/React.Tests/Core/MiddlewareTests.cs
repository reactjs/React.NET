#if NETCOREAPP2_0

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
			environment.Setup(x => x.WebRootPath).Returns("c:\\temp");
			Assert.Equal("c:\\temp\\wwwroot\\script.js", new AspNetFileSystem(environment.Object).MapPath("~/wwwroot/script.js"));
		}
	}
}
#endif
