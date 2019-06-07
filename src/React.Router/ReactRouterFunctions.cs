using System;

namespace React.Router
{
	/// <summary>
	/// Render functions for React Router
	/// </summary>
	public class ReactRouterFunctions : RenderFunctionsBase
	{
		/// <summary>
		/// The returned react router context, as a JSON string
		/// A default value wards off deserialization exceptions
		/// when server side rendering is disabled
		/// </summary>
		public string ReactRouterContext { get; private set; } = "{}";

		/// <summary>
		/// Implementation of PreRender
		/// </summary>
		/// <param name="executeJs"></param>
		public override void PreRender(Func<string, string> executeJs)
		{
			executeJs("var context = {};");
		}

		/// <summary>
		/// Implementation of PostRender
		/// </summary>
		/// <param name="executeJs"></param>
		public override void PostRender(Func<string, string> executeJs)
		{
			ReactRouterContext = executeJs("JSON.stringify(context);");
		}
	}
}
