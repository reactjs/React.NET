/*
 *  Copyright (c) 2014-Present, Facebook, Inc.
 *  All rights reserved.
 *
 *  This source code is licensed under the BSD-style license found in the
 *  LICENSE file in the root directory of this source tree. An additional grant
 *  of patent rights can be found in the PATENTS file in the same directory.
 */
#if NET452 || NETCOREAPP2_0

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
			var reactIdGenerator = new Mock<IReactIdGenerator>();

			var component = new ReactRouterComponent(environment.Object, config.Object, reactIdGenerator.Object, "Foo", "container", "/bar")
			{
				Props = new { hello = "World" }
			};
			var result = component.RenderJavaScript();

			Assert.Equal(
				@"ReactDOM.hydrate(React.createElement(Foo, {""hello"":""World""}), document.getElementById(""container""))",
				result
			);
		}
	}
}
#endif
