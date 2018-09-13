using System;

namespace React.RenderFunctions
{
	/// <summary>
	/// 
	/// </summary>
	public abstract class RenderFunctionsBase : IRenderFunctions
	{
		private readonly IRenderFunctions m_renderFunctions;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="renderFunctions"></param>
		protected RenderFunctionsBase(IRenderFunctions renderFunctions)
		{
			m_renderFunctions = renderFunctions;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="executeJs"></param>
		protected abstract void PreRenderCore(Func<string, string> executeJs);

		/// <summary>
		/// 
		/// </summary>
		/// <param name="componentToRender"></param>
		/// <returns></returns>
		protected abstract string TransformRenderCore(string componentToRender);

		/// <summary>
		/// 
		/// </summary>
		/// <param name="executeJs"></param>
		protected abstract void PostRenderCore(Func<string, string> executeJs);

		/// <summary>
		/// 
		/// </summary>
		/// <param name="executeJs"></param>
		public virtual void PreRender(Func<string, string> executeJs)
		{
			m_renderFunctions?.PreRender(executeJs);
			PreRenderCore(executeJs);
		}


		/// <summary>
		/// 
		/// </summary>
		/// <param name="componentToRender"></param>
		/// <returns></returns>
		public string TransformRender(string componentToRender)
		{
			return m_renderFunctions == null
				? componentToRender
				: m_renderFunctions.TransformRender(TransformRenderCore(componentToRender));
		}


		/// <summary>
		/// 
		/// </summary>
		/// <param name="executeJs"></param>
		public void PostRender(Func<string, string> executeJs)
		{
			PostRenderCore(executeJs);
			m_renderFunctions?.PostRender(executeJs);
		}
	}
}
