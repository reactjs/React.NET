using System;

namespace React
{
	/// <summary>
	/// Functions to execute during a render request.
	/// These functions will share the same Javascript context, so state can be passed around via variables.
	/// </summary>
	public abstract class RenderFunctions
	{
		private readonly RenderFunctions m_renderFunctions;

		/// <summary>
		/// Constructor. Supports chained calls to multiple render functions by passing in a set of functions that should be called next.
		/// The functions within the provided RenderFunctions will be called *after* this instance's.
		/// Supports null as an argument.
		/// </summary>
		/// <param name="renderFunctions">The chained render functions to call</param>
		protected RenderFunctions(RenderFunctions renderFunctions)
		{
			m_renderFunctions = renderFunctions;
		}

		/// <summary>
		/// Implementation of PreRender
		/// </summary>
		/// <param name="executeJs"></param>
		protected virtual void PreRenderCore(Func<string, string> executeJs)
		{
		}

		/// <summary>
		/// Implementation of TransformRender
		/// </summary>
		/// <param name="componentToRender"></param>
		/// <returns></returns>
		protected virtual string TransformRenderCore(string componentToRender)
		{
			return componentToRender;
		}

		/// <summary>
		/// Implementation of PostRender
		/// </summary>
		/// <param name="executeJs"></param>
		protected virtual void PostRenderCore(Func<string, string> executeJs)
		{
		}

		/// <summary>
		/// Executes before component render.
		/// It takes a func that accepts a Javascript code expression to evaluate, which returns the result of the expression.
		/// This is useful for setting up variables that will be referenced after the render completes.
		/// <param name="executeJs">The func to execute</param>
		/// </summary>
		public virtual void PreRender(Func<string, string> executeJs)
		{
			PreRenderCore(executeJs);
			m_renderFunctions?.PreRender(executeJs);
		}


		/// <summary>
		/// Transforms the React.createElement expression.
		/// This is useful for libraries like styled components which require wrapping the root component
		/// inside a helper to generate a stylesheet.
		/// Example transform: React.createElement(Foo, ...) => wrapComponent(React.createElement(Foo, ...))
		/// </summary>
		/// <param name="componentToRender">The Javascript expression to wrap</param>
		/// <returns>A wrapped expression</returns>
		public string TransformRender(string componentToRender)
		{
			return m_renderFunctions == null
				? TransformRenderCore(componentToRender)
				: m_renderFunctions.TransformRender(TransformRenderCore(componentToRender));
		}


		/// <summary>
		/// Executes after component render.
		/// It takes a func that accepts a Javascript code expression to evaluate, which returns the result of the expression.
		/// This is useful for reading computed state, such as generated stylesheets or a router redirect result.
		/// </summary>
		/// <param name="executeJs">The func to execute</param>
		public void PostRender(Func<string, string> executeJs)
		{
			PostRenderCore(executeJs);
			m_renderFunctions?.PostRender(executeJs);
		}
	}
}
