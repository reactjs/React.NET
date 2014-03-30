using Moq;
using React.Exceptions;
using Xunit;

namespace React.Tests.Core
{
	public class ReactComponentTest
	{
		[Fact]
		public void RenderHtmlShouldThrowExceptionIfComponentDoesNotExist()
		{
			var environment = new Mock<IReactEnvironment>();
			environment.Setup(x => x.HasVariable("Foo")).Returns(false);
			var component = new ReactComponent(environment.Object, "Foo", "container");

			Assert.Throws<ReactInvalidComponentException>(() =>
			{
				component.RenderHtml();
			});
		}

		[Fact]
		public void RenderHtmlShouldCallRenderComponent()
		{
			var environment = new Mock<IReactEnvironment>();
			environment.Setup(x => x.HasVariable("Foo")).Returns(true);

			var component = new ReactComponent(environment.Object, "Foo", "container")
			{
				Props = new { hello = "World" }
			};
			component.RenderHtml();

			environment.Verify(x => x.Execute<string>(@"React.renderComponentToString(Foo({""hello"":""World""}))"));
		}

		[Fact]
		public void RenderHtmlShouldWrapComponentInDiv()
		{
			var environment = new Mock<IReactEnvironment>();
			environment.Setup(x => x.HasVariable("Foo")).Returns(true);
			environment.Setup(x => x.Execute<string>(@"React.renderComponentToString(Foo({""hello"":""World""}))"))
				.Returns("[HTML]");

			var component = new ReactComponent(environment.Object, "Foo", "container")
			{
				Props = new { hello = "World" }
			};
			var result = component.RenderHtml();

			Assert.Equal(@"<div id=""container"">[HTML]</div>", result);
		}

		[Fact]
		public void RenderJavaScriptShouldCallRenderComponent()
		{
			var environment = new Mock<IReactEnvironment>();

			var component = new ReactComponent(environment.Object, "Foo", "container")
			{
				Props = new { hello = "World" }
			};
			var result = component.RenderJavaScript();

			Assert.Equal(
				@"React.renderComponent(Foo({""hello"":""World""}), document.getElementById(""container""))",
				result
			);
		}
	}
}
