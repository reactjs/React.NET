﻿/*
 *  Copyright (c) 2014-2015, Facebook, Inc.
 *  All rights reserved.
 *
 *  This source code is licensed under the BSD-style license found in the
 *  LICENSE file in the root directory of this source tree. An additional grant 
 *  of patent rights can be found in the PATENTS file in the same directory.
 */

using System.Runtime.Remoting;
using Moq;
using NUnit.Framework;
using React.Web.Mvc;

namespace React.Tests.Mvc
{
	[TestFixture]
	class HtmlHelperExtensionsTests
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

		[Test]
		public void ReactWithInitShouldReturnHtmlAndScript()
		{
			var component = new Mock<IReactComponent>();
			component.Setup(x => x.RenderHtml()).Returns("HTML");
			component.Setup(x => x.RenderJavaScript()).Returns("JS");
			var environment = ConfigureMockEnvironment();
			environment.Setup(x => x.CreateComponent(
				"ComponentName",
				new {},
				null
			)).Returns(component.Object);

			var result = HtmlHelperExtensions.ReactWithInit(
				htmlHelper: null,
				componentName: "ComponentName", 
				props: new { }, 
				htmlTag: "span"
			);
			Assert.AreEqual(
				"HTML" + System.Environment.NewLine + "<script>JS</script>", 
				result.ToString()
			);

		}
	}
}
