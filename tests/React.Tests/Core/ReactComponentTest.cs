/*
 * Copyright (c) Facebook, Inc. and its affiliates.
 *
 * This source code is licensed under the MIT license found in the
 * LICENSE file in the root directory of this source tree.
 */

using System;
using JavaScriptEngineSwitcher.Core;
using Moq;
using React.Exceptions;
using React.RenderFunctions;
using Xunit;

namespace React.Tests.Core
{
	public class ReactComponentTest
	{
		[Fact]
		public void RenderHtmlShouldThrowExceptionIfComponentDoesNotExist()
		{
			var environment = new Mock<IReactEnvironment>();
			environment.Setup(x => x.Execute<bool>("typeof Foo !== 'undefined'")).Returns(false);
			var config = CreateDefaultConfigMock();
			config.Setup(x => x.UseServerSideRendering).Returns(true);
			var reactIdGenerator = new Mock<IReactIdGenerator>();

			var component = new ReactComponent(environment.Object, config.Object, reactIdGenerator.Object, "Foo", "container");

			Assert.Throws<ReactInvalidComponentException>(() =>
			{
				component.RenderHtml();
			});
		}

		[Fact]
		public void RenderHtmlShouldCallRenderComponent()
		{
			var environment = new Mock<IReactEnvironment>();
			environment.Setup(x => x.Execute<bool>("typeof Foo !== 'undefined'")).Returns(true);
			var config = CreateDefaultConfigMock();
			config.Setup(x => x.UseServerSideRendering).Returns(true);
			var reactIdGenerator = new Mock<IReactIdGenerator>();

			var component = new ReactComponent(environment.Object, config.Object, reactIdGenerator.Object, "Foo", "container")
			{
				Props = new { hello = "World" }
			};
			component.RenderHtml();

			environment.Verify(x => x.Execute<string>(@"ReactDOMServer.renderToString(React.createElement(Foo, {""hello"":""World""}))"));
		}

		[Fact]
		public void RenderHtmlShouldWrapComponentInDiv()
		{
			var environment = new Mock<IReactEnvironment>();
			environment.Setup(x => x.Execute<bool>("typeof Foo !== 'undefined'")).Returns(true);
			environment.Setup(x => x.Execute<string>(@"ReactDOMServer.renderToString(React.createElement(Foo, {""hello"":""World""}))"))
				.Returns("[HTML]");
			var config = CreateDefaultConfigMock();
			config.Setup(x => x.UseServerSideRendering).Returns(true);
			var reactIdGenerator = new Mock<IReactIdGenerator>();

			var component = new ReactComponent(environment.Object, config.Object, reactIdGenerator.Object, "Foo", "container")
			{
				Props = new { hello = "World" }
			};
			var result = component.RenderHtml();

			Assert.Equal(@"<div id=""container"">[HTML]</div>", result);
		}

		[Fact]
		public void RenderHtmlShouldNotRenderComponentHtml()
		{
			var environment = new Mock<IReactEnvironment>();
			environment.Setup(x => x.Execute<bool>("typeof Foo !== 'undefined'")).Returns(true);
			environment.Setup(x => x.Execute<string>(@"React.renderToString(React.createElement(Foo, {""hello"":""World""}))"))
				.Returns("[HTML]");
			var config = CreateDefaultConfigMock();
			var reactIdGenerator = new Mock<IReactIdGenerator>();

			var component = new ReactComponent(environment.Object, config.Object, reactIdGenerator.Object, "Foo", "container")
			{
				Props = new { hello = "World" }
			};
			var result = component.RenderHtml(renderContainerOnly: true);

			Assert.Equal(@"<div id=""container""></div>", result);
			environment.Verify(x => x.Execute(It.IsAny<string>()), Times.Never);
		}

		[Fact]
		public void RenderHtmlShouldNotRenderClientSideAttributes()
		{
			var environment = new Mock<IReactEnvironment>();
			environment.Setup(x => x.Execute<bool>("typeof Foo !== 'undefined'")).Returns(true);
			var config = CreateDefaultConfigMock();
			config.Setup(x => x.UseServerSideRendering).Returns(true);
			var reactIdGenerator = new Mock<IReactIdGenerator>();

			var component = new ReactComponent(environment.Object, config.Object, reactIdGenerator.Object, "Foo", "container")
			{
				Props = new { hello = "World" }
			};
			component.RenderHtml(renderServerOnly: true);

			environment.Verify(x => x.Execute<string>(@"ReactDOMServer.renderToStaticMarkup(React.createElement(Foo, {""hello"":""World""}))"));
		}

		[Fact]
		public void RenderHtmlShouldWrapComponentInCustomElement()
		{
			var config = CreateDefaultConfigMock();
			config.Setup(x => x.UseServerSideRendering).Returns(true);
			var environment = new Mock<IReactEnvironment>();
			environment.Setup(x => x.Execute<bool>("typeof Foo !== 'undefined'")).Returns(true);
			environment.Setup(x => x.Execute<string>(@"ReactDOMServer.renderToString(React.createElement(Foo, {""hello"":""World""}))"))
				.Returns("[HTML]");
			var reactIdGenerator = new Mock<IReactIdGenerator>();

			var component = new ReactComponent(environment.Object, config.Object, reactIdGenerator.Object, "Foo", "container")
			{
				Props = new { hello = "World" },
				ContainerTag = "span"
			};
			var result = component.RenderHtml();

			Assert.Equal(@"<span id=""container"">[HTML]</span>", result);
		}

		[Fact]
		public void RenderHtmlShouldNotRenderComponentWhenContainerOnly()
		{
			var config = CreateDefaultConfigMock();
			config.Setup(x => x.UseServerSideRendering).Returns(true);
			var environment = new Mock<IReactEnvironment>();
			environment.Setup(x => x.Execute<bool>("typeof Foo !== 'undefined'")).Returns(true);
			environment.Setup(x => x.Execute<string>(@"ReactDOMServer.renderToString(React.createElement(Foo, {""hello"":""World""}))"))
				.Returns("[HTML]");
			var reactIdGenerator = new Mock<IReactIdGenerator>();

			var component = new ReactComponent(environment.Object, config.Object, reactIdGenerator.Object, "Foo", "container")
			{
				Props = new { hello = "World" },
				ContainerTag = "span"
			};
			var result = component.RenderHtml(true, false);

			Assert.Equal(@"<span id=""container""></span>", result);
		}

		[Fact]
		public void RenderHtmlShouldNotWrapComponentWhenServerSideOnly()
		{
			var config = CreateDefaultConfigMock();
			config.Setup(x => x.UseServerSideRendering).Returns(true);
			var environment = new Mock<IReactEnvironment>();
			environment.Setup(x => x.Execute<bool>("typeof Foo !== 'undefined'")).Returns(true);
			environment.Setup(x => x.Execute<string>(@"ReactDOMServer.renderToStaticMarkup(React.createElement(Foo, {""hello"":""World""}))"))
				.Returns("[HTML]");
			var reactIdGenerator = new Mock<IReactIdGenerator>();

			var component = new ReactComponent(environment.Object, config.Object, reactIdGenerator.Object, "Foo", "container")
			{
				Props = new { hello = "World" },
			};
			var result = component.RenderHtml(false, true);

			Assert.Equal(@"[HTML]", result);
		}

		[Fact]
		public void RenderHtmlShouldAddClassToElement()
		{
			var config = CreateDefaultConfigMock();
			config.Setup(x => x.UseServerSideRendering).Returns(true);
			var environment = new Mock<IReactEnvironment>();
			environment.Setup(x => x.Execute<bool>("typeof Foo !== 'undefined'")).Returns(true);
			environment.Setup(x => x.Execute<string>(@"ReactDOMServer.renderToString(React.createElement(Foo, {""hello"":""World""}))"))
				.Returns("[HTML]");

			var reactIdGenerator = new Mock<IReactIdGenerator>();

			var component = new ReactComponent(environment.Object, config.Object, reactIdGenerator.Object, "Foo", "container")
			{
				Props = new { hello = "World" },
				ContainerClass = "test-class"
			};
			var result = component.RenderHtml();

			Assert.Equal(@"<div id=""container"" class=""test-class"">[HTML]</div>", result);
		}

		[Fact]
		public void RenderJavaScriptShouldCallRenderComponent()
		{
			var environment = new Mock<IReactEnvironment>();
			var config = CreateDefaultConfigMock();
			var reactIdGenerator = new Mock<IReactIdGenerator>();

			var component = new ReactComponent(environment.Object, config.Object, reactIdGenerator.Object, "Foo", "container")
			{
				Props = new { hello = "World" }
			};
			var result = component.RenderJavaScript();

			Assert.Equal(
				@"ReactDOM.hydrate(React.createElement(Foo, {""hello"":""World""}), document.getElementById(""container""))",
				result
			);
		}

		[Fact]
		public void RenderJavaScriptShouldCallRenderComponentWithReactDOMRender()
		{
			var environment = new Mock<IReactEnvironment>();
			var config = CreateDefaultConfigMock();
			var reactIdGenerator = new Mock<IReactIdGenerator>();

			var component = new ReactComponent(environment.Object, config.Object, reactIdGenerator.Object, "Foo", "container")
			{
				ClientOnly = true,
				Props = new { hello = "World" }
			};
			var result = component.RenderJavaScript();

			Assert.Equal(
				@"ReactDOM.render(React.createElement(Foo, {""hello"":""World""}), document.getElementById(""container""))",
				result
			);
		}

		[Fact]
		public void RenderJavaScriptShouldCallRenderComponentwithReactDOMHydrate()
		{
			var environment = new Mock<IReactEnvironment>();
			var config = CreateDefaultConfigMock();
			var reactIdGenerator = new Mock<IReactIdGenerator>();

			var component = new ReactComponent(environment.Object, config.Object, reactIdGenerator.Object, "Foo", "container")
			{
				ClientOnly = false,
				Props = new { hello = "World" }
			};
			var result = component.RenderJavaScript();

			Assert.Equal(
				@"ReactDOM.hydrate(React.createElement(Foo, {""hello"":""World""}), document.getElementById(""container""))",
				result
			);
		}

		[Fact]
		public void RenderJavaScriptShouldCallRenderComponentWithReactDomRenderWhenSsrDisabled()
		{
			var environment = new Mock<IReactEnvironment>();
			var config = CreateDefaultConfigMock();
			config.SetupGet(x => x.UseServerSideRendering).Returns(false);

			var reactIdGenerator = new Mock<IReactIdGenerator>();
			var component = new ReactComponent(environment.Object, config.Object, reactIdGenerator.Object, "Foo", "container")
			{
				ClientOnly = false,
				Props = new {hello = "World"}
			};
			var result = component.RenderJavaScript();
			
			Assert.Equal(
				@"ReactDOM.render(React.createElement(Foo, {""hello"":""World""}), document.getElementById(""container""))",
				result
			);
		}

		[Theory]
		[InlineData("Foo", true)]
		[InlineData("Foo.Bar", true)]
		[InlineData("Foo.Bar.Baz", true)]
		[InlineData("alert()", false)]
		[InlineData("Foo.alert()", false)]
		[InlineData("lol what", false)]
		public void TestEnsureComponentNameValid(string input, bool expected)
		{
			var isValid = true;
			try
			{
				ReactComponent.EnsureComponentNameValid(input);
			}
			catch (ReactInvalidComponentException)
			{
				isValid = false;
			}
			Assert.Equal(expected, isValid);
		}


		[Fact]
		public void GeneratesContainerIdIfNotProvided()
		{
			var environment = new Mock<IReactEnvironment>();
			var config = CreateDefaultConfigMock();
			var reactIdGenerator = new Mock<IReactIdGenerator>();
			reactIdGenerator.Setup(x => x.Generate()).Returns("customReactId");

			var component = new ReactComponent(environment.Object, config.Object, reactIdGenerator.Object, "Foo", null);
			Assert.Equal("customReactId", component.ContainerId);
		}

		[Fact]
		public void ExceptionThrownIsHandled()
		{
			var environment = new Mock<IReactEnvironment>();
			environment.Setup(x => x.Execute<bool>("typeof Foo !== 'undefined'")).Returns(true);
			environment.Setup(x => x.Execute<string>(@"ReactDOMServer.renderToString(React.createElement(Foo, {""hello"":""World""}))"))
				.Throws(new JsRuntimeException("'undefined' is not an object"));

			var config = CreateDefaultConfigMock();
			config.Setup(x => x.UseServerSideRendering).Returns(true);
			config.Setup(x => x.ExceptionHandler).Returns(() => throw new ReactServerRenderingException("test"));

			var reactIdGenerator = new Mock<IReactIdGenerator>();

			var component = new ReactComponent(environment.Object, config.Object, reactIdGenerator.Object, "Foo", "container")
			{
				Props = new { hello = "World" }
			};

			// Default behavior
			bool exceptionCaught = false;
			try
			{
				component.RenderHtml();
			}
			catch (ReactServerRenderingException)
			{
				exceptionCaught = true;
			}

			Assert.True(exceptionCaught);

			// Custom handler passed into render call
			bool customHandlerInvoked = false;
			Action<Exception, string, string> customHandler = (ex, name, id) => customHandlerInvoked = true;
			component.RenderHtml(exceptionHandler: customHandler);
			Assert.True(customHandlerInvoked);

			// Custom exception handler set
			Exception caughtException = null;
			config.Setup(x => x.ExceptionHandler).Returns((ex, name, id) => caughtException = ex);

			var result = component.RenderHtml();
			Assert.Equal(@"<div id=""container""></div>", result);
			Assert.NotNull(caughtException);
		}

		[Fact]
		public void RenderFunctionsCalled()
		{
			var environment = new Mock<IReactEnvironment>();
			environment.Setup(x => x.Execute<bool>("typeof Foo !== 'undefined'")).Returns(true);
			environment.Setup(x => x.Execute<string>(@"outerWrap(ReactDOMServer.renderToString(wrap(React.createElement(Foo, {""hello"":""World""}))))"))
				.Returns("[HTML]");

			environment.Setup(x => x.Execute<string>(@"prerender();"))
				.Returns("prerender-result");

			environment.Setup(x => x.Execute<string>(@"postrender();"))
				.Returns("postrender-result");

			var config = CreateDefaultConfigMock();
			config.Setup(x => x.UseServerSideRendering).Returns(true);
			var reactIdGenerator = new Mock<IReactIdGenerator>();

			var component = new ReactComponent(environment.Object, config.Object, reactIdGenerator.Object, "Foo", "container")
			{
				Props = new { hello = "World" }
			};
			var renderFunctions = new TestRenderFunctions();
			var result = component.RenderHtml(renderFunctions: renderFunctions);

			Assert.Equal(@"<div id=""container"">[HTML]</div>", result);
			Assert.Equal(@"prerender-result", renderFunctions.PreRenderResult);
			Assert.Equal(@"postrender-result", renderFunctions.PostRenderResult);
		}

		[Fact]
		public void ChainedRenderFunctionsCalled()
		{
			var firstInstance = new TestRenderFunctions();
			var secondInstance = new TestRenderFunctions();
			var chainedRenderFunctions = new ChainedRenderFunctions(firstInstance, secondInstance);

			chainedRenderFunctions.PreRender(a => "prerender-result");

			Assert.Equal("prerender-result", firstInstance.PreRenderResult);
			Assert.Equal("prerender-result", secondInstance.PreRenderResult);

			string wrapComponentResult = chainedRenderFunctions.WrapComponent("React.createElement('div', null)");
			Assert.Equal("wrap(wrap(React.createElement('div', null)))", wrapComponentResult);

			Assert.Equal("outerWrap(input)", firstInstance.TransformRenderedHtml("input"));
			Assert.Equal("outerWrap(outerWrap(input))", chainedRenderFunctions.TransformRenderedHtml("input"));

			chainedRenderFunctions.PostRender(a => "postrender-result");

			Assert.Equal("postrender-result", firstInstance.PostRenderResult);
			Assert.Equal("postrender-result", secondInstance.PostRenderResult);
		}
		
		private static Mock<IReactSiteConfiguration> CreateDefaultConfigMock()
		{
			var configMock = new Mock<IReactSiteConfiguration>();
			configMock.SetupGet(x => x.UseServerSideRendering).Returns(true);
			return configMock;
		}

		private sealed class TestRenderFunctions : RenderFunctionsBase
		{
			public string PreRenderResult { get; private set; }
			public string PostRenderResult { get; private set; }

			public override void PreRender(Func<string, string> executeJs)
			{
				PreRenderResult = executeJs("prerender();");
			}

			public override string WrapComponent(string componentToRender)
			{
				return $"wrap({componentToRender})";
			}

			public override string TransformRenderedHtml(string input)
			{
				return $"outerWrap({input})";
			}

			public override void PostRender(Func<string, string> executeJs)
			{
				PostRenderResult = executeJs("postrender();");
			}
		}
	}
}
