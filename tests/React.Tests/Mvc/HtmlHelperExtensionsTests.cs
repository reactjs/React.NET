/*
 * Copyright (c) Facebook, Inc. and its affiliates.
 *
 * This source code is licensed under the MIT license found in the
 * LICENSE file in the root directory of this source tree.
 */
#if NET452

using System;
using System.IO;
using System.Security.Cryptography;
using Moq;
using React.Web.Mvc;
using Xunit;

namespace React.Tests.Mvc
{
	public class HtmlHelperExtensionsTests
	{
		/// <summary>
		/// Creates a mock <see cref="IReactEnvironment"/> and registers it with the IoC container
		/// This is only required because <see cref="HtmlHelperExtensions"/> can not be
		/// injected :(
		/// </summary>
		private Mock<IReactEnvironment> ConfigureMockEnvironment(IReactSiteConfiguration configuration = null)
		{
			var environment = new Mock<IReactEnvironment>();
			environment.Setup(x => x.Configuration).Returns(configuration ?? new ReactSiteConfiguration());
			AssemblyRegistration.Container.Register(environment.Object);
			return environment;
		}

		[Fact]
		public void ReactWithInitShouldReturnHtmlAndScript()
		{
			var component = new Mock<IReactComponent>();

			component.Setup(x => x.RenderHtml(It.IsAny<TextWriter>(), false, false, null, null))
				.Callback((TextWriter writer, bool renderContainerOnly, bool renderServerOnly, Action<Exception, string, string> exceptionHandler, IRenderFunctions renderFunctions) => writer.Write("HTML"));

			component.Setup(x => x.RenderJavaScript(It.IsAny<TextWriter>())).Callback((TextWriter writer) => writer.Write("JS"));

			var environment = ConfigureMockEnvironment();
			environment.Setup(x => x.CreateComponent(
				"ComponentName",
				new { },
				null,
				false,
				false
			)).Returns(component.Object);

			var result = HtmlHelperExtensions.ReactWithInit(
				htmlHelper: null,
				componentName: "ComponentName",
				props: new { },
				htmlTag: "span"
			).ToHtmlString();

			Assert.Equal(
				"HTML" + System.Environment.NewLine + "<script>JS</script>",
				result.ToString()
			);
		}

		[Fact]
		public void ScriptNonceIsReturned()
		{
			string nonce;
			using (var random = new RNGCryptoServiceProvider())
			{
				byte[] nonceBytes = new byte[16];
				random.GetBytes(nonceBytes);
				nonce = Convert.ToBase64String(nonceBytes);
			}

			var component = new Mock<IReactComponent>();
			component.Setup(x => x.RenderHtml(It.IsAny<TextWriter>(), false, false, null, null))
				.Callback((TextWriter writer, bool renderContainerOnly, bool renderServerOnly, Action<Exception, string, string> exceptionHandle, IRenderFunctions renderFunctions) => writer.Write("HTML")).Verifiable();

			component.Setup(x => x.RenderJavaScript(It.IsAny<TextWriter>())).Callback((TextWriter writer) => writer.Write("JS")).Verifiable();

			var config = new Mock<IReactSiteConfiguration>();

			var environment = ConfigureMockEnvironment(config.Object);

			environment.Setup(x => x.Configuration).Returns(config.Object);
			environment.Setup(x => x.CreateComponent(
				"ComponentName",
				new { },
				null,
				false,
				false
			)).Returns(component.Object);

			// without nonce
			var result = HtmlHelperExtensions.ReactWithInit(
				htmlHelper: null,
				componentName: "ComponentName",
				props: new { },
				htmlTag: "span"
			).ToHtmlString();

			Assert.Equal(
				"HTML" + System.Environment.NewLine + "<script>JS</script>",
				result.ToString()
			);

			config.Setup(x => x.ScriptNonceProvider).Returns(() => nonce);

			// with nonce
			result = HtmlHelperExtensions.ReactWithInit(
				htmlHelper: null,
				componentName: "ComponentName",
				props: new { },
				htmlTag: "span"
			).ToHtmlString();

			Assert.Equal(
				"HTML" + System.Environment.NewLine + "<script nonce=\"" + nonce + "\">JS</script>",
				result.ToString()
			);
		}

		[Fact]
		public void EngineIsReturnedToPoolAfterRender()
		{
			var component = new Mock<IReactComponent>();
			component.Setup(x => x.RenderHtml(It.IsAny<TextWriter>(), false, false, null, null))
				.Callback((TextWriter writer, bool renderContainerOnly, bool renderServerOnly, Action<Exception, string, string> exceptionHandler, IRenderFunctions renderFunctions) => writer.Write("HTML")).Verifiable();

			var environment = ConfigureMockEnvironment();
			environment.Setup(x => x.CreateComponent(
				"ComponentName",
				new { },
				null,
				true,
				false
			)).Returns(component.Object);

			environment.Verify(x => x.ReturnEngineToPool(), Times.Never);
			var result = HtmlHelperExtensions.React(
				htmlHelper: null,
				componentName: "ComponentName",
				props: new { },
				htmlTag: "span",
				clientOnly: true,
				serverOnly: false
			).ToHtmlString();

			component.Verify(x => x.RenderHtml(It.IsAny<TextWriter>(), It.Is<bool>(y => y == true), It.Is<bool>(z => z == false), null, null), Times.Once);
			environment.Verify(x => x.ReturnEngineToPool(), Times.Once);
		}

		[Fact]
		public void ReactWithClientOnlyTrueShouldCallRenderHtmlWithTrue()
		{
			var component = new Mock<IReactComponent>();
			component.Setup(x => x.RenderHtml(It.IsAny<TextWriter>(), false, false, null, null))
				.Callback((TextWriter writer, bool renderContainerOnly, bool renderServerOnly, Action<Exception, string, string> exceptionHandler, IRenderFunctions renderFunctions) => writer.Write("HTML")).Verifiable();

			var environment = ConfigureMockEnvironment();
			environment.Setup(x => x.CreateComponent(
				"ComponentName",
				new { },
				null,
				true,
				false
			)).Returns(component.Object);

			var result = HtmlHelperExtensions.React(
				htmlHelper: null,
				componentName: "ComponentName",
				props: new { },
				htmlTag: "span",
				clientOnly: true,
				serverOnly: false
			).ToHtmlString();

			component.Verify(x => x.RenderHtml(It.IsAny<TextWriter>(), It.Is<bool>(y => y == true), It.Is<bool>(z => z == false), null, null), Times.Once);
		}

		[Fact]
		public void ReactWithServerOnlyTrueShouldCallRenderHtmlWithTrue()
		{
			var component = new Mock<IReactComponent>();
			component.Setup(x => x.RenderHtml(It.IsAny<TextWriter>(), false, false, null, null))
				.Callback((TextWriter writer, bool renderContainerOnly, bool renderServerOnly, Action<Exception, string, string> exceptionHandler, IRenderFunctions renderFunctions) => writer.Write("HTML")).Verifiable();

			var environment = ConfigureMockEnvironment();
			environment.Setup(x => x.CreateComponent(
				"ComponentName",
				new { },
				null,
				false,
				true
			)).Returns(component.Object);

			var result = HtmlHelperExtensions.React(
				htmlHelper: null,
				componentName: "ComponentName",
				props: new { },
				htmlTag: "span",
				clientOnly: false,
				serverOnly: true
			).ToHtmlString();

			component.Verify(x => x.RenderHtml(It.IsAny<TextWriter>(), It.Is<bool>(y => y == false), It.Is<bool>(z => z == true), null, null), Times.Once);
		}

		[Fact]
		public void RenderFunctionsCalledNonLazily()
		{
			var component = new Mock<IReactComponent>();
			var fakeRenderFunctions = new Mock<IRenderFunctions>();
			fakeRenderFunctions.Setup(x => x.PreRender(It.IsAny<Func<string, string>>())).Verifiable();
			fakeRenderFunctions.Setup(x => x.PostRender(It.IsAny<Func<string, string>>())).Verifiable();
			fakeRenderFunctions.Setup(x => x.TransformRenderedHtml(It.IsAny<string>())).Returns("HTML");

			component.Setup(x => x.RenderHtml(It.IsAny<TextWriter>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<Action<Exception, string, string>>(), It.IsAny<IRenderFunctions>()))
				.Callback((TextWriter writer, bool renderContainerOnly, bool renderServerOnly, Action<Exception, string, string> exceptionHandler, IRenderFunctions renderFunctions) => 
				{
					renderFunctions.PreRender(_ => "one");
					writer.Write(renderFunctions.TransformRenderedHtml("HTML"));
					renderFunctions.PostRender(_ => "two");
				}).Verifiable();

			var environment = ConfigureMockEnvironment();
			environment.Setup(x => x.CreateComponent(
				"ComponentName",
				new { },
				null,
				false,
				true
			)).Returns(component.Object);

			var result = HtmlHelperExtensions.React(
				htmlHelper: null,
				componentName: "ComponentName",
				props: new { },
				htmlTag: "span",
				clientOnly: false,
				serverOnly: true,
				renderFunctions: fakeRenderFunctions.Object
			);

			// JS calls must happen right away so thrown exceptions do not crash the app.
			component.Verify(x => x.RenderHtml(It.IsAny<TextWriter>(), It.Is<bool>(y => y == false), It.Is<bool>(z => z == true), It.IsAny<Action<Exception, string, string>>(), It.IsAny<IRenderFunctions>()), Times.Once);
			fakeRenderFunctions.Verify(x => x.PreRender(It.IsAny<Func<string, string>>()), Times.Once);
			fakeRenderFunctions.Verify(x => x.PostRender(It.IsAny<Func<string, string>>()), Times.Once);

			Assert.Equal("HTML", result.ToHtmlString());
		}
	}
}
#endif
