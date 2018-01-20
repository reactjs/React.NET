/*
 *  Copyright (c) 2014-Present, Facebook, Inc.
 *  All rights reserved.
 *
 *  This source code is licensed under the BSD-style license found in the
 *  LICENSE file in the root directory of this source tree. An additional grant
 *  of patent rights can be found in the PATENTS file in the same directory.
 */

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
		private Mock<IReactEnvironment> ConfigureMockEnvironment()
		{
			var environment = new Mock<IReactEnvironment>();
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
				new {},
				null,
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
		public void EngineIsReturnedToPoolAfterRender()
		{
			var component = new Mock<IReactComponent>();
			component.Setup(x => x.RenderHtml(true, true, null)).Returns("HTML");
			var environment = ConfigureMockEnvironment();
			environment.Setup(x => x.CreateComponent(
				"ComponentName",
				new { },
				null,
				true
			)).Returns(component.Object);

			environment.Verify(x => x.ReturnEngineToPool(), Times.Never);
			var result = HtmlHelperExtensions.React(
				htmlHelper: null,
				componentName: "ComponentName",
				props: new { },
				htmlTag: "span",
				clientOnly: true,
				serverOnly: true
			);
			component.Verify(x => x.RenderHtml(It.Is<bool>(y => y == true), It.Is<bool>(z => z == true), null), Times.Once);
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
				new {},
				null,
				true
			)).Returns(component.Object);

			var result = HtmlHelperExtensions.React(
				htmlHelper: null,
				componentName: "ComponentName",
				props: new { },
				htmlTag: "span",
				clientOnly: true,
				serverOnly: true
			);
			component.Verify(x => x.RenderHtml(It.Is<bool>(y => y == true), It.Is<bool>(z => z == true), null), Times.Once);
		}

		[Fact]
		public void ReactWithServerOnlyTrueShouldCallRenderHtmlWithTrue() {
			var component = new Mock<IReactComponent>();
			component.Setup(x => x.RenderHtml(true, true, null)).Returns("HTML");
			var environment = ConfigureMockEnvironment();
			environment.Setup(x => x.CreateComponent(
				"ComponentName",
				new { },
				null,
				true
			)).Returns(component.Object);

			var result = HtmlHelperExtensions.React(
				htmlHelper: null,
				componentName: "ComponentName",
				props: new { },
				htmlTag: "span",
				clientOnly: true,
				serverOnly: true
			);
			component.Verify(x => x.RenderHtml(It.Is<bool>(y => y == true), It.Is<bool>(z => z == true), null), Times.Once);
		}
	}
}
