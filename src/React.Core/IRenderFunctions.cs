using System;

namespace React
{
	/// <summary>
	/// Functions to execute during a render request.
	/// These functions will share the same Javascript context, so state can be passed around via variables.
	/// </summary>
	public interface IRenderFunctions
	{
		/// <summary>
		/// Executes before component render.
		/// It takes a func that accepts a Javascript code expression to evaluate, which returns the result of the expression.
		/// This is useful for setting up variables that will be referenced after the render completes.
		/// <param name="executeJs">The func to execute</param>
		/// </summary>
		void PreRender(Func<string, string> executeJs);


		/// <summary>
		/// Transforms the React.createElement expression.
		/// This is useful for libraries like styled components which require wrapping the root component
		/// inside a helper to generate a stylesheet.
		/// Example transform: React.createElement(Foo, ...) => wrapComponent(React.createElement(Foo, ...))
		/// </summary>
		/// <param name="componentToRender">The Javascript expression to wrap</param>
		/// <returns>A wrapped expression</returns>
		string WrapComponent(string componentToRender);


		/// <summary>
		/// Transforms the compiled rendered component HTML
		/// This is useful for libraries like emotion which take rendered component HTML and output the transformed HTML plus additional style tags
		/// </summary>
		/// <param name="input">The component HTML</param>
		/// <returns>A wrapped expression</returns>
		string TransformRenderedHtml(string input);


		/// <summary>
		/// Executes after component render.
		/// It takes a func that accepts a Javascript code expression to evaluate, which returns the result of the expression.
		/// This is useful for reading computed state, such as generated stylesheets or a router redirect result.
		/// </summary>
		/// <param name="executeJs">The func to execute</param>
		void PostRender(Func<string, string> executeJs);
	}
}
