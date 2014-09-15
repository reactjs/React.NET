/*
 *  Copyright (c) 2014, Facebook, Inc.
 *  All rights reserved.
 *
 *  This source code is licensed under the BSD-style license found in the
 *  LICENSE file in the root directory of this source tree. An additional grant 
 *  of patent rights can be found in the PATENTS file in the same directory.
 */

using Moq;
using NUnit.Framework;
using React.Exceptions;

namespace React.Tests.Core
{
	[TestFixture]
	public class ReactComponentTest
	{
		[Test]
		public void RenderHtmlShouldThrowExceptionIfComponentDoesNotExist()
		{
			var environment = new Mock<IReactEnvironment>();
			environment.Setup(x => x.Execute<bool>("typeof Foo !== 'undefined'")).Returns(false);
			var component = new ReactComponent(environment.Object, "Foo", "container");

			Assert.Throws<ReactInvalidComponentException>(() =>
			{
				component.RenderHtml();
			});
		}

		[Test]
		public void RenderHtmlShouldCallRenderComponent()
		{
			var environment = new Mock<IReactEnvironment>();
			environment.Setup(x => x.Execute<bool>("typeof Foo !== 'undefined'")).Returns(true);

			var component = new ReactComponent(environment.Object, "Foo", "container")
			{
				Props = new { hello = "World" }
			};
			component.RenderHtml();

			environment.Verify(x => x.Execute<string>(@"React.renderComponentToString(Foo({""hello"":""World""}))"));
		}

		[Test]
		public void RenderHtmlShouldWrapComponentInDiv()
		{
			var environment = new Mock<IReactEnvironment>();
			environment.Setup(x => x.Execute<bool>("typeof Foo !== 'undefined'")).Returns(true);
			environment.Setup(x => x.Execute<string>(@"React.renderComponentToString(Foo({""hello"":""World""}))"))
				.Returns("[HTML]");

			var component = new ReactComponent(environment.Object, "Foo", "container")
			{
				Props = new { hello = "World" }
			};
			var result = component.RenderHtml();

			Assert.AreEqual(@"<div id=""container"">[HTML]</div>", result);
		}

		[Test]
		public void RenderJavaScriptShouldCallRenderComponent()
		{
			var environment = new Mock<IReactEnvironment>();

			var component = new ReactComponent(environment.Object, "Foo", "container")
			{
				Props = new { hello = "World" }
			};
			var result = component.RenderJavaScript();

			Assert.AreEqual(
				@"React.renderComponent(Foo({""hello"":""World""}), document.getElementById(""container""))",
				result
			);
		}

		[TestCase("Foo", true)]
		[TestCase("Foo.Bar", true)]
		[TestCase("Foo.Bar.Baz", true)]
		[TestCase("alert()", false)]
		[TestCase("Foo.alert()", false)]
		[TestCase("lol what", false)]
		public void TestEnsureComponentNameValid(string input, bool expected)
		{
			var isValid = true;
			try
			{
				ReactComponent.EnsureComponentNameValid(input);
			}
			catch (ReactInvalidComponentException)
			{
				isValid =  false;
			}
			Assert.AreEqual(expected, isValid);
		}
	}
}
