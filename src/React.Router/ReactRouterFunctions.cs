using System;
using React.RenderFunctions;

namespace React.Router
{
	/// <summary>
	/// 
	/// </summary>
	public class ReactRouterFunctions : RenderFunctionsBase
	{
		private Action<string> _onPostRender;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="renderFunctions"></param>
		/// <param name="onPostRender"></param>
		public ReactRouterFunctions(IRenderFunctions renderFunctions = null, Action<string> onPostRender = null)
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
			executeJs("var context = {};");
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="componentToRender"></param>
		/// <returns></returns>
		protected override string TransformRenderCore(string componentToRender)
		{

			return componentToRender;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="executeJs"></param>
		protected override void PostRenderCore(Func<string, string> executeJs)
		{
			_onPostRender(executeJs("JSON.stringify(context);"));
		}
	}
}
