using System;

namespace React.Router
{
	/// <summary>
	/// Render functions for React Router
	/// </summary>
	public class ReactRouterFunctions : RenderFunctions
	{
		/// <summary>
		/// Constructor. Supports chained calls to multiple render functions by passing in a set of functions that should be called next.
		/// The functions within the provided RenderFunctions will be called *after* this instance's.
		/// Supports null as an argument.
		/// </summary>
		/// <param name="renderFunctions">The chained render functions to call</param>
		public ReactRouterFunctions(RenderFunctions renderFunctions = null)
			: base(renderFunctions)
		{
		}

		/// <summary>
		/// The returned react router context, as a JSON string
		/// </summary>
		public string ReactRouterContext { get; private set; }

		/// <summary>
		/// Implementation of PreRender
		/// </summary>
		/// <param name="executeJs"></param>
		protected override void PreRenderCore(Func<string, string> executeJs)
		{
			executeJs("var context = {};");
		}

		/// <summary>
		/// Implementation of PostRender
		/// </summary>
		/// <param name="executeJs"></param>
		protected override void PostRenderCore(Func<string, string> executeJs)
		{
			ReactRouterContext = executeJs("JSON.stringify(context);");	
		}
	}
}
