using System;

namespace React
{
	/// <summary>
	/// Render functions for React-JSS
	/// </summary>
	public class ReactJssFunctions : RenderFunctions
	{
		/// <summary>
		/// Constructor. Supports chained calls to multiple render functions by passing in a set of functions that should be called next.
		/// The functions within the provided RenderFunctions will be called *after* this instance's.
		/// Supports null as an argument.
		/// </summary>
		/// <param name="renderFunctions">The chained render functions to call</param>
		public ReactJssFunctions(RenderFunctions renderFunctions = null)
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
			executeJs("var reactJssProps = { registry: new ReactJss.SheetsRegistry() };");
		}

		/// <summary>
		/// Implementation of TransformRender
		/// </summary>
		/// <param name="componentToRender"></param>
		/// <returns></returns>
		protected override string TransformRenderCore(string componentToRender)
		{
			return ($"React.createElement(ReactJss.JssProvider, reactJssProps, ({componentToRender}))");
		}

		/// <summary>
		/// Implementation of PostRender
		/// </summary>
		/// <param name="executeJs"></param>
		protected override void PostRenderCore(Func<string, string> executeJs)
		{
			RenderedStyles = $"<style type=\"text/css\" id=\"server-side-styles\">{executeJs("reactJssProps.registry.toString()")}</style>";
		}
	}
}
