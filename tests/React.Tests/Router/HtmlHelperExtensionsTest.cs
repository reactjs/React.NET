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

		/// <summary>
		/// Mock an html helper with a mocked response object.
		/// Used when testing for server response modification.
		/// </summary>
		class HtmlHelperMocks
		{
			public Mock<HtmlHelper> htmlHelper;
			public Mock<HttpResponseBase> httpResponse;

			public HtmlHelperMocks()
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

		/// <summary>
		/// Mocks alot of common functionality related to rendering a 
		/// React Router component.
		/// </summary>
		class ReactRouterMocks
		{
			public Mock<IReactSiteConfiguration> config;
			public Mock<IReactEnvironment> environment;
			public Mock<ReactRouterComponent> component;

			public ReactRouterMocks(
				Mock<IReactSiteConfiguration> conf,
				Mock<IReactEnvironment> env
			)
			{
				config = conf;
				environment = env;

				component = new Mock<ReactRouterComponent>(
					environment.Object,
					config.Object,
					"ComponentName",
					"",
					"/"
				);
				var execResult = new Mock<ExecutionResult>();

				component.Setup(x => x.RenderRouterWithContext(It.IsAny<bool>(), It.IsAny<bool>()))
					.Returns(execResult.Object);
				environment.Setup(x => x.CreateComponent(
					It.IsAny<IReactComponent>(),
					It.IsAny<bool>()
				)).Returns(component.Object);
				environment.Setup(x => x.Execute<string>("JSON.stringify(context);"))
							.Returns("{ }");
			}
		}

		[Fact]
		public void EngineIsReturnedToPoolAfterRender()
		{
			var config = ConfigureMockConfiguration();
			var environment = ConfigureMockEnvironment();
			var routerMocks = new ReactRouterMocks(config, environment);
			var htmlHelperMock = new HtmlHelperMocks();

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

		[Fact]
		public void ReactWithClientOnlyTrueShouldCallRenderHtmlWithTrue()
		{
			var config = ConfigureMockConfiguration();

			var htmlHelperMock = new HtmlHelperMocks();
			var environment = ConfigureMockEnvironment();
			var routerMocks = new ReactRouterMocks(config, environment);

			var result = HtmlHelperExtensions.ReactRouterWithContext(
				htmlHelper: htmlHelperMock.htmlHelper.Object,
				componentName: "ComponentName",
				props: new { },
				path: "/",
				htmlTag: "span",
				clientOnly: true,
				serverOnly: false
			);
			routerMocks.component.Verify(x => x.RenderRouterWithContext(It.Is<bool>(y => y == true), It.Is<bool>(z => z == false)), Times.Once);
		}

		[Fact]
		public void ReactWithServerOnlyTrueShouldCallRenderHtmlWithTrue()
		{
			var config = ConfigureMockConfiguration();

			var htmlHelperMock = new HtmlHelperMocks();
			var environment = ConfigureMockEnvironment();
			var routerMocks = new ReactRouterMocks(config, environment);

			var result = HtmlHelperExtensions.ReactRouterWithContext(
				htmlHelper: htmlHelperMock.htmlHelper.Object,
				componentName: "ComponentName",
				props: new { },
				path: "/",
				htmlTag: "span",
				clientOnly: false,
				serverOnly: true
			);
			routerMocks.component.Verify(x => x.RenderRouterWithContext(It.Is<bool>(y => y == false), It.Is<bool>(z => z == true)), Times.Once);
		}

		[Fact]
		public void ShouldModifyStatusCode()
		{
			var mocks = ConfigureMockReactEnvironment();
			ConfigureMockConfiguration();

			mocks.Engine.Setup(x => x.Evaluate<string>("JSON.stringify(context);"))
						.Returns("{ status: 200 }");

			var htmlHelperMock = new HtmlHelperMocks();

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

			var htmlHelperMock = new HtmlHelperMocks();

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

			var htmlHelperMock = new HtmlHelperMocks();

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

			var htmlHelperMock = new HtmlHelperMocks();

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
