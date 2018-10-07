using System;

namespace React.StylesheetHelpers
{
	/// <summary>
	/// Render functions for styled components. https://github.com/styled-components/styled-components
	/// Requires `styled-components` to be exposed globally as `Styled`
	/// </summary>
	public class StyledComponentsFunctions : RenderFunctions
	{
		/// <summary>
		/// Constructor. Supports chained calls to multiple render functions by passing in a set of functions that should be called next.
		/// The functions within the provided RenderFunctions will be called *after* this instance's.
		/// Supports null as an argument.
		/// </summary>
		/// <param name="renderFunctions">The chained render functions to call</param>
		public StyledComponentsFunctions(RenderFunctions renderFunctions = null)
			: base(renderFunctions)
		{
		}
		
		/// <summary>
		/// HTML style tag containing the rendered styles
		/// </summary>
		public string RenderedStyles { get; private set; }

		/// <summary>
		/// Implementation of PreRender
		/// </summary>
		/// <param name="executeJs"></param>
		protected override void PreRenderCore(Func<string, string> executeJs)
		{
			executeJs("var serverStyleSheet = new Styled.ServerStyleSheet();");
		}

		/// <summary>
		/// Implementation of WrapComponent
		/// </summary>
		/// <param name="componentToRender"></param>
		/// <returns></returns>
		protected override string WrapComponentCore(string componentToRender)
		{
			return ($"serverStyleSheet.collectStyles({componentToRender})");
		}

		/// <summary>
		/// Implementation of PostRender
		/// </summary>
		/// <param name="executeJs"></param>
		protected override void PostRenderCore(Func<string, string> executeJs)
		{
			RenderedStyles = executeJs("serverStyleSheet.getStyleTags()");
		}
	}
}
