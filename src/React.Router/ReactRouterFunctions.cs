using System;
using React.RenderFunctions;

namespace React.Router
{
	public class ReactRouterFunctions : RenderFunctionsBase
	{
		private Action<string> _onPostRender;

		public ReactRouterFunctions(IRenderFunctions renderFunctions = null, Action<string> onPostRender = null)
			: base(renderFunctions)
		{
			_onPostRender = onPostRender;
		}

		protected override void PreRenderCore(Func<string, string> executeJs)
		{
			executeJs("var context = {};");
		}

		protected override string TransformRenderCore(string componentToRender)
		{

			return componentToRender;
		}

		protected override void PostRenderCore(Func<string, string> executeJs)
		{
			_onPostRender(executeJs("JSON.stringify(context);"));
		}
	}
}
