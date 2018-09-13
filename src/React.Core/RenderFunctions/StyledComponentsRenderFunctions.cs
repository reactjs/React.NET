using System;

namespace React.RenderFunctions
{
	public class StyledComponentsFunctions : RenderFunctionsBase
	{
		private Action<string> _onPostRender;

		public StyledComponentsFunctions(IRenderFunctions renderFunctions = null, Action<string> onPostRender = null)
			: base(renderFunctions)
		{
			_onPostRender = onPostRender;
		}

		protected override void PreRenderCore(Func<string, string> executeJs)
		{
			executeJs("var serverStyleSheet = new Styled.ServerStyleSheet();");
		}

		protected override string TransformRenderCore(string componentToRender)
		{
			return ($"serverStyleSheet.collectStyles({componentToRender})");
		}

		protected override void PostRenderCore(Func<string, string> executeJs)
		{
			_onPostRender(executeJs("serverStyleSheet.getStyleTags()"));
		}
	}
}
