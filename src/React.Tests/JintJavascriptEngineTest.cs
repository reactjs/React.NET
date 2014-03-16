using React;

namespace React.Tests.Core
{
	public class JintJavascriptEngineTest : JavascriptEngineTestBase
	{
		protected override IJavascriptEngine CreateEngine()
		{
			return new JintJavascriptEngine();
		}
	}
}
