using System;

namespace React.Router
{
	/// <summary>
	///
	/// </summary>
	public class ReactRouterFunctions : RenderFunctions
	{
		/// <summary>
		///
		/// </summary>
		/// <param name="renderFunctions"></param>
		public ReactRouterFunctions(RenderFunctions renderFunctions = null)
			: base(renderFunctions)
		{
		}

		/// <summary>
		/// The returned react router context, as a JSON string
		/// </summary>
		public string ReactRouterContext { get; private set; }

		/// <summary>
		///
		/// </summary>
		/// <param name="executeJs"></param>
		protected override void PreRenderCore(Func<string, string> executeJs)
		{
			executeJs("var context = {};");
		}

		/// <summary>
		///
		/// </summary>
		/// <param name="executeJs"></param>
		protected override void PostRenderCore(Func<string, string> executeJs)
		{
			ReactRouterContext = executeJs("JSON.stringify(context);");	
		}
	}
}
