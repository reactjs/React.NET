using React;
using Xunit;

namespace React.Tests.Core
{
    public abstract class JavascriptEngineTestBase
    {
	    protected abstract IJavascriptEngine CreateEngine();

		[Fact]
	    public void SetVariableShouldSetInGlobalScope()
		{
			var engine = CreateEngine();
			engine.SetVariable("foo", "Hello World");
			Assert.Equal("Hello World", engine.Execute<string>("foo"));
		}

		[Fact]
	    public void ExecuteFunctionShouldHandleOneArgument()
	    {
			var engine = CreateEngine();
			engine.Execute(@"
function foo(arg1) {
	return arg1;
}");
			var result = engine.ExecuteFunction<string>("foo", "Hello World");
			Assert.Equal("Hello World", result);
	    }

		[Fact]
		public void ExecuteFunctionShouldHandleTwoArguments()
		{
			var engine = CreateEngine();
			engine.Execute(@"
function add(first, second) {
	return first + second;
}");
			var result = engine.ExecuteFunction<double>("add", 10D, 5D);
			Assert.Equal(15D, result);
		}
    }
}
