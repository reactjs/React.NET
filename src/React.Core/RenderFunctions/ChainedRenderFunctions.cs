using System;
using System.Text;

namespace React.RenderFunctions
{
	/// <summary>
	/// Helper to chain functions to be executed during server-side rendering.
	/// For instance, React Router and React Helmet can both be used together using this class.
	/// </summary>
	public class ChainedRenderFunctions : IRenderFunctions
	{
		private readonly IRenderFunctions[] _chainedFunctions;

		/// <summary>
		/// Constructor. Supports chained calls to multiple render functions by passing in a set of functions that should be called next.
		/// </summary>
		/// <param name="chainedFunctions">The chained render functions to call</param>
		public ChainedRenderFunctions(params IRenderFunctions[] chainedFunctions)
		{
			_chainedFunctions = chainedFunctions;
		}

		/// <summary>
		/// Executes before component render.
		/// It takes a func that accepts a Javascript code expression to evaluate, which returns the result of the expression.
		/// This is useful for setting up variables that will be referenced after the render completes.
		/// <param name="executeJs">The func to execute</param>
		/// </summary>
		public void PreRender(Func<string, string> executeJs)
		{
			foreach (var chainedFunction in _chainedFunctions)
			{
				chainedFunction.PreRender(executeJs);
			}
		}


		/// <summary>
		/// Transforms the React.createElement expression.
		/// This is useful for libraries like styled components which require wrapping the root component
		/// inside a helper to generate a stylesheet.
		/// Example transform: React.createElement(Foo, ...) => wrapComponent(React.createElement(Foo, ...))
		/// </summary>
		/// <param name="componentToRender">The Javascript expression to wrap</param>
		/// <returns>A wrapped expression</returns>
		public string WrapComponent(string componentToRender)
		{
			string wrappedComponent = componentToRender;

			foreach (var chainedFunction in _chainedFunctions)
			{
				wrappedComponent = chainedFunction.WrapComponent(wrappedComponent);
			}

			return wrappedComponent;
		}


		/// <summary>
		/// Transforms the compiled rendered component HTML
		/// This is useful for libraries like emotion which take rendered component HTML and output the transformed HTML plus additional style tags
		/// </summary>
		/// <param name="input">The component HTML</param>
		/// <returns>A wrapped expression</returns>
		public string TransformRenderedHtml(string input)
		{
			string renderedHtml = input;

			foreach (var chainedFunction in _chainedFunctions)
			{
				renderedHtml = chainedFunction.TransformRenderedHtml(renderedHtml);
			}

			return renderedHtml;
		}


		/// <summary>
		/// Executes after component render.
		/// It takes a func that accepts a Javascript code expression to evaluate, which returns the result of the expression.
		/// This is useful for reading computed state, such as generated stylesheets or a router redirect result.
		/// </summary>
		/// <param name="executeJs">The func to execute</param>
		public void PostRender(Func<string, string> executeJs)
		{
			foreach (var chainedFunction in _chainedFunctions)
			{
				chainedFunction.PostRender(executeJs);
			}
		}
	}
}
