using System;

namespace React.RenderFunctions
{
	/// <summary>
	/// Render functions for React-JSS. https://github.com/cssinjs/react-jss
	/// Requires `react-jss` to be exposed globally as `ReactJss`
	/// </summary>
	public class ReactJssFunctions : RenderFunctionsBase
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
			executeJs("var reactJssProps = { registry: new ReactJss.SheetsRegistry() };");
		}

		/// <summary>
		/// Implementation of WrapComponent
		/// </summary>
		/// <param name="componentToRender"></param>
		/// <returns></returns>
		public override string WrapComponent(string componentToRender)
		{
			return ($"React.createElement(ReactJss.JssProvider, reactJssProps, ({componentToRender}))");
		}

		/// <summary>
		/// Implementation of PostRender
		/// </summary>
		/// <param name="executeJs"></param>
		public override void PostRender(Func<string, string> executeJs)
		{
			RenderedStyles = $"<style type=\"text/css\" id=\"server-side-styles\">{executeJs("reactJssProps.registry.toString()")}</style>";
		}
	}
}
