using System;

namespace React
{
	/// <summary>
	/// Render functions for styled components
	/// </summary>
	public class StyledComponentsFunctions : RenderFunctions
	{
		/// <summary>
		///
		/// </summary>
		/// <param name="renderFunctions"></param>
		public StyledComponentsFunctions(RenderFunctions renderFunctions = null)
			: base(renderFunctions)
		{
		}
		
		/// <summary>
		/// A HTML style tag containing the rendered styles
		/// </summary>
		public string RenderedStyles { get; private set; }

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
			RenderedStyles = executeJs("serverStyleSheet.getStyleTags()");
		}
	}
}
