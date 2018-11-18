using System;

namespace React.RenderFunctions
{
	/// <summary>
	/// Render functions for styled components. https://github.com/styled-components/styled-components
	/// Requires `styled-components` to be exposed globally as `Styled`
	/// </summary>
	public class StyledComponentsFunctions : RenderFunctionsBase
	{
		/// <summary>
		/// HTML style tag containing the rendered styles
		/// </summary>
		public string RenderedStyles { get; private set; }

		/// <summary>
		/// Implementation of PreRender
		/// </summary>
		/// <param name="executeJs"></param>
		public override void PreRender(Func<string, string> executeJs)
		{
			executeJs("var serverStyleSheet = new Styled.ServerStyleSheet();");
		}

		/// <summary>
		/// Implementation of WrapComponent
		/// </summary>
		/// <param name="componentToRender"></param>
		/// <returns></returns>
		public override string WrapComponent(string componentToRender)
		{
			return ($"serverStyleSheet.collectStyles({componentToRender})");
		}

		/// <summary>
		/// Implementation of PostRender
		/// </summary>
		/// <param name="executeJs"></param>
		public override void PostRender(Func<string, string> executeJs)
		{
			RenderedStyles = executeJs("serverStyleSheet.getStyleTags()");
		}
	}
}
