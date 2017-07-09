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
using System.Web;
using JavaScriptEngineSwitcher.Core;
using System.Web.Mvc;

namespace React.Tests.Router
{
	public class HtmlHelperExtensionsTest
	{
		public interface IReactEnvironmentMock : IReactEnvironment
		{
			IJsEngine Engine { get; set; }
		}

		/// <summary>
		/// Creates a mock <see cref="IReactEnvironment"/> and registers it with the IoC container
		/// This is only required because <see cref="HtmlHelperExtensions"/> can not be
		/// injected :(
		/// </summary>
		private ReactEnvironmentTest.Mocks ConfigureMockReactEnvironment()
		{
			var mocks = new ReactEnvironmentTest.Mocks();

			var environment = mocks.CreateReactEnvironment();
			AssemblyRegistration.Container.Register<IReactEnvironment>(environment);
			return mocks;
		}

		private Mock<IReactEnvironment> ConfigureMockEnvironment()
		{
			var environment = new Mock<IReactEnvironment>();
			AssemblyRegistration.Container.Register(environment.Object);
			return environment;
		}

		private Mock<IReactSiteConfiguration> ConfigureMockConfiguration()
		{
			var config = new Mock<IReactSiteConfiguration>();
			AssemblyRegistration.Container.Register(config.Object);
			return config;
		}

		class HtmlHelperMock
		{
			public Mock<HtmlHelper> htmlHelper;
			public Mock<HttpResponseBase> httpResponse;

			public HtmlHelperMock()
			{
				var viewDataContainer = new Mock<IViewDataContainer>();
				var viewContext = new Mock<ViewContext>();
				httpResponse = new Mock<HttpResponseBase>();
				htmlHelper = new Mock<HtmlHelper>(viewContext.Object, viewDataContainer.Object);
				var httpContextBase = new Mock<HttpContextBase>();
				viewContext.Setup(x => x.HttpContext).Returns(httpContextBase.Object);
				httpContextBase.Setup(x => x.Response).Returns(httpResponse.Object);
			}
		}

		[Fact]
		public void EngineIsReturnedToPoolAfterRender()
		{
			ConfigureMockConfiguration();

			var component = new Mock<IReactComponent>();
			component.Setup(x => x.RenderHtml(true, true)).Returns("HTML");

			var environment = ConfigureMockEnvironment();
			environment.Setup(x => x.CreateComponent(
				"ComponentName",
				new { },
				null,
				true
			)).Returns(component.Object);
			environment.Setup(x => x.Execute<string>("JSON.stringify(context);"))
						.Returns("{ }");

			var htmlHelperMock = new HtmlHelperMock();

			environment.Verify(x => x.ReturnEngineToPool(), Times.Never);
			var result = HtmlHelperExtensions.ReactRouterWithContext(
				htmlHelper: htmlHelperMock.htmlHelper.Object,
				componentName: "ComponentName",
				props: new { },
				path: "/",
				htmlTag: "span",
				clientOnly: true,
				serverOnly: true
			);
			environment.Verify(x => x.ReturnEngineToPool(), Times.Once);
		}

		//[Fact]
		//public void ReactWithClientOnlyTrueShouldCallRenderHtmlWithTrue()
		//{
		//	var component = new Mock<IReactComponent>();
		//	component.Setup(x => x.RenderHtml(true, true)).Returns("HTML");
		//	var environment = ConfigureMockEnvironment();
		//	environment.Setup(x => x.CreateComponent(
		//		"ComponentName",
		//		new { },
		//		null,
		//		true
		//	)).Returns(component.Object);

		//	var result = HtmlHelperExtensions.ReactRouterWithContext(
		//		htmlHelper: null,
		//		componentName: "ComponentName",
		//		props: new { },
		//		htmlTag: "span",
		//		clientOnly: true,
		//		serverOnly: true
		//	);
		//	component.Verify(x => x.RenderHtml(It.Is<bool>(y => y == true), It.Is<bool>(z => z == true)), Times.Once);
		//}

		//[Fact]
		//public void ReactWithServerOnlyTrueShouldCallRenderHtmlWithTrue()
		//{
		//	var component = new Mock<IReactComponent>();
		//	component.Setup(x => x.RenderHtml(true, true)).Returns("HTML");
		//	var environment = ConfigureMockEnvironment();
		//	environment.Setup(x => x.CreateComponent(
		//		"ComponentName",
		//		new { },
		//		null,
		//		true
		//	)).Returns(component.Object);

		//	var result = HtmlHelperExtensions.React(
		//		htmlHelper: null,
		//		componentName: "ComponentName",
		//		props: new { },
		//		htmlTag: "span",
		//		clientOnly: true,
		//		serverOnly: true
		//	);
		//	component.Verify(x => x.RenderHtml(It.Is<bool>(y => y == true), It.Is<bool>(z => z == true)), Times.Once);
		//}

		[Fact]
		public void ShouldModifyStatusCode()
		{
			var mocks = ConfigureMockReactEnvironment();
			ConfigureMockConfiguration();

			mocks.Engine.Setup(x => x.Evaluate<string>("JSON.stringify(context);"))
						.Returns("{ status: 200 }");

			var htmlHelperMock = new HtmlHelperMock();

			HtmlHelperExtensions.ReactRouterWithContext(
				htmlHelper: htmlHelperMock.htmlHelper.Object,
				componentName: "ComponentName",
				props: new { },
				path: "/"
			);
			htmlHelperMock.httpResponse.VerifySet(x => x.StatusCode = 200);
		}

		[Fact]
		public void ShouldRunCustomContextHandler()
		{
			var mocks = ConfigureMockReactEnvironment();
			ConfigureMockConfiguration();

			mocks.Engine.Setup(x => x.Evaluate<string>("JSON.stringify(context);"))
						.Returns("{ status: 200 }");

			var htmlHelperMock = new HtmlHelperMock();

			HtmlHelperExtensions.ReactRouterWithContext(
				htmlHelper: htmlHelperMock.htmlHelper.Object,
				componentName: "ComponentName",
				props: new { },
				path: "/",
				contextHandler: (response, context) => response.StatusCode = context.status.Value
			);
			htmlHelperMock.httpResponse.VerifySet(x => x.StatusCode = 200);
		}

		[Fact]
		public void ShouldRedirectPermanent()
		{
			var mocks = ConfigureMockReactEnvironment();
			ConfigureMockConfiguration();

			mocks.Engine.Setup(x => x.Evaluate<string>("JSON.stringify(context);"))
						.Returns(@"{ status: 301, url: ""/foo"" }");

			var htmlHelperMock = new HtmlHelperMock();

			HtmlHelperExtensions.ReactRouterWithContext(
				htmlHelper: htmlHelperMock.htmlHelper.Object,
				componentName: "ComponentName",
				props: new { },
				path: "/"
			);
			htmlHelperMock.httpResponse.Verify(x => x.RedirectPermanent(It.IsAny<string>()));
		}

		[Fact]
		public void ShouldFailRedirectWithNoUrl()
		{
			var mocks = ConfigureMockReactEnvironment();
			ConfigureMockConfiguration();

			mocks.Engine.Setup(x => x.Evaluate<string>("JSON.stringify(context);"))
						.Returns("{ status: 301 }");

			var htmlHelperMock = new HtmlHelperMock();

			Assert.Throws<ReactRouterException>(() =>
			
				HtmlHelperExtensions.ReactRouterWithContext(
					htmlHelper: htmlHelperMock.htmlHelper.Object,
					componentName: "ComponentName",
					props: new { },
					path: "/"
				)
			);
		}
	}
}
