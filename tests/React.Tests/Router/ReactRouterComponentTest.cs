/*
 *  Copyright (c) 2014-Present, Facebook, Inc.
 *  All rights reserved.
 *
 *  This source code is licensed under the BSD-style license found in the
 *  LICENSE file in the root directory of this source tree. An additional grant
 *  of patent rights can be found in the PATENTS file in the same directory.
 */

using Moq;
using Xunit;
using React.Exceptions;
using React.Router;
using React.Tests.Core;

namespace React.Tests.Router
{
	public class ReactRouterComponentTest
	{
		[Fact]
		public void RenderJavaScriptShouldNotIncludeContextOrPath()
		{
			var environment = new Mock<IReactEnvironment>();
			var config = new Mock<IReactSiteConfiguration>();

			var component = new ReactRouterComponent(environment.Object, config.Object, "Foo", "container", "/bar")
			{
				Props = new { hello = "World" }
			};
			var result = component.RenderJavaScript();

			Assert.Equal(
				@"ReactDOM.render(React.createElement(Foo, {""hello"":""World""}), document.getElementById(""container""))",
				result
			);
		}

		//[Fact]
		//public void ShouldUsePolifyfillSuccessfully()
		//{
		//    var mocks = new ReactEnvironmentTest.Mocks();
		//    var environment = mocks.CreateReactEnvironment();
		//    //environment.Setup(x => x.Execute<bool>("typeof Object.assign === 'function'")).Returns(false);
		//    //environment.Setup(x => x.Execute<string>("JSON.stringify(context);")).Returns("{}");

		//    var config = new Mock<IReactSiteConfiguration>();
		//    config.Setup(x => x.UseServerSideRendering).Returns(false);

		//    var component = new ReactRouterComponent(environment, config.Object, "Foo", "container", "/bar")
		//    {
		//        Props = new { hello = "World" }
		//    };
		//    var html = component.RenderRouterWithContext(true);
		//    var result = environment.Execute<string>(@"Object.assign(""a"", { sup=""yo""})");
		//    Assert.Equal(@"{ [string: ""a""], sup: ""yo""", result);
		//}
	}
}
