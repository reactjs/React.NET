using React;

namespace React.Tests.Core
{
	public class MsieJavaScriptEngineTest : JavascriptEngineTestBase
	{
		protected override IJavascriptEngine CreateEngine()
		{
			return new MsieJavascriptEngine();
		}
	}
}
