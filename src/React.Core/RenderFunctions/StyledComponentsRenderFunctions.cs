using System;

namespace React.RenderFunctions
{
	/// <summary>
	/// 
	/// </summary>
	public class StyledComponentsFunctions : RenderFunctionsBase
	{
		private Action<string> _onPostRender;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="renderFunctions"></param>
		/// <param name="onPostRender"></param>
		public StyledComponentsFunctions(IRenderFunctions renderFunctions = null, Action<string> onPostRender = null)
			: base(renderFunctions)
		{
			_onPostRender = onPostRender;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="executeJs"></param>
		protected override void PreRenderCore(Func<string, string> executeJs)
		{
			executeJs("var serverStyleSheet = new Styled.ServerStyleSheet();");
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="componentToRender"></param>
		/// <returns></returns>
		protected override string TransformRenderCore(string componentToRender)
		{
			return ($"serverStyleSheet.collectStyles({componentToRender})");
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="executeJs"></param>
		protected override void PostRenderCore(Func<string, string> executeJs)
		{
			_onPostRender(executeJs("serverStyleSheet.getStyleTags()"));
		}
	}
}
