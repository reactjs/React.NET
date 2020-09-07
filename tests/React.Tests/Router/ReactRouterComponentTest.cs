/*
 * Copyright (c) Facebook, Inc. and its affiliates.
 *
 * This source code is licensed under the MIT license found in the
 * LICENSE file in the root directory of this source tree.
 */
#if NET452 || NETCOREAPP

using Moq;
using Xunit;
using React.Router;

namespace React.Tests.Router
{
	public class ReactRouterComponentTest
	{
		[Theory]
		[InlineData(true, true)]
		[InlineData(true, false)]
		[InlineData(false, true)]
		[InlineData(false, false)]
		public void RenderJavaScriptShouldNotIncludeContextOrPath(bool clientOnly, bool useServerSideRendering)
		{
			var environment = new Mock<IReactEnvironment>();
			var config = new Mock<IReactSiteConfiguration>();
			config.Setup(x => x.UseServerSideRendering).Returns(useServerSideRendering);
			var reactIdGenerator = new Mock<IReactIdGenerator>();

			var component = new ReactRouterComponent(environment.Object, config.Object, reactIdGenerator.Object, "Foo", "container", "/bar")
			{
				Props = new { hello = "World" },
				ClientOnly = clientOnly,
			};
			var result = component.RenderJavaScript(false);

			if (clientOnly || !useServerSideRendering)
			{
				Assert.Equal(
					@"ReactDOM.render(React.createElement(Foo, {""hello"":""World""}), document.getElementById(""container""))",
					result
				);
			}
			else
			{
				Assert.Equal(
					@"ReactDOM.hydrate(React.createElement(Foo, {""hello"":""World""}), document.getElementById(""container""))",
					result
				);
			}

		}

		[Theory]
		[InlineData(true, true)]
		[InlineData(true, false)]
		[InlineData(false, true)]
		[InlineData(false, false)]
		public void RenderJavaScriptShouldHandleWaitForContentLoad(bool clientOnly, bool useServerSideRendering)
		{
			var environment = new Mock<IReactEnvironment>();
			var config = new Mock<IReactSiteConfiguration>();
			config.Setup(x => x.UseServerSideRendering).Returns(useServerSideRendering);
			var reactIdGenerator = new Mock<IReactIdGenerator>();

			var component = new ReactRouterComponent(environment.Object, config.Object, reactIdGenerator.Object, "Foo", "container", "/bar")
			{
				Props = new { hello = "World" },
				ClientOnly = clientOnly
			};
			var result = component.RenderJavaScript(true);

			if (clientOnly || !useServerSideRendering)
			{
				Assert.Equal(
					@"window.addEventListener('DOMContentLoaded', function() {ReactDOM.render(React.createElement(Foo, {""hello"":""World""}), document.getElementById(""container""))});",
					result
				);
			}
			else
			{
				Assert.Equal(
					@"window.addEventListener('DOMContentLoaded', function() {ReactDOM.hydrate(React.createElement(Foo, {""hello"":""World""}), document.getElementById(""container""))});",
					result
				);
			}
		}
	}
}
#endif
