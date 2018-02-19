/*
 *  Copyright (c) 2014-Present, Facebook, Inc.
 *  All rights reserved.
 *
 *  This source code is licensed under the BSD-style license found in the
 *  LICENSE file in the root directory of this source tree. An additional grant
 *  of patent rights can be found in the PATENTS file in the same directory.
 */

using System;
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
			component.Setup(x => x.RenderHtml(false, false, null)).Returns("HTML");
			component.Setup(x => x.RenderJavaScript()).Returns("JS");
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
			);
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
			component.Setup(x => x.RenderHtml(false, false, null)).Returns("HTML");
			component.Setup(x => x.RenderJavaScript()).Returns("JS");

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
			);
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
			);
			Assert.Equal(
				"HTML" + System.Environment.NewLine + "<script nonce=\"" + nonce + "\">JS</script>",
				result.ToString()
			);
		}

		[Fact]
		public void EngineIsReturnedToPoolAfterRender()
		{
			var component = new Mock<IReactComponent>();
			component.Setup(x => x.RenderHtml(true, true, null)).Returns("HTML");
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
			);
			component.Verify(x => x.RenderHtml(It.Is<bool>(y => y == true), It.Is<bool>(z => z == false), null), Times.Once);
			environment.Verify(x => x.ReturnEngineToPool(), Times.Once);
		}

		[Fact]
		public void ReactWithClientOnlyTrueShouldCallRenderHtmlWithTrue()
		{
			var component = new Mock<IReactComponent>();
			component.Setup(x => x.RenderHtml(true, true, null)).Returns("HTML");
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
			);
			component.Verify(x => x.RenderHtml(It.Is<bool>(y => y == true), It.Is<bool>(z => z == false), null), Times.Once);
		}

		[Fact]
		public void ReactWithServerOnlyTrueShouldCallRenderHtmlWithTrue()
		{
			var component = new Mock<IReactComponent>();
			component.Setup(x => x.RenderHtml(false, true, null)).Returns("HTML");
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
			);
			component.Verify(x => x.RenderHtml(It.Is<bool>(y => y == false), It.Is<bool>(z => z == true), null), Times.Once);
		}
	}
}
