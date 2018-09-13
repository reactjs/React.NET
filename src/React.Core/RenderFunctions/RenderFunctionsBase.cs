using System;

namespace React.RenderFunctions
{
	public abstract class RenderFunctionsBase : IRenderFunctions
	{
		private readonly IRenderFunctions m_renderFunctions;

		protected RenderFunctionsBase(IRenderFunctions renderFunctions)
		{
			m_renderFunctions = renderFunctions;
		}

		protected abstract void PreRenderCore(Func<string, string> executeJs);
		protected abstract string TransformRenderCore(string componentToRender);
		protected abstract void PostRenderCore(Func<string, string> executeJs);

		public virtual void PreRender(Func<string, string> executeJs)
		{
			m_renderFunctions?.PreRender(executeJs);
			PreRenderCore(executeJs);
		}


		public string TransformRender(string componentToRender)
		{
			return m_renderFunctions == null
				? componentToRender
				: m_renderFunctions.TransformRender(TransformRenderCore(componentToRender));
		}


		public void PostRender(Func<string, string> executeJs)
		{
			PostRenderCore(executeJs);
			m_renderFunctions?.PostRender(executeJs);
		}
	}
}
