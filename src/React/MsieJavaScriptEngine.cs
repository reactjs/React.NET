using System.Reflection;
using MsieJavaScriptEngine;

namespace React
{
	/// <summary>
	/// Handles executing JavaScript using the Internet Explorer JavaScript engine. Requires 
	/// Internet Explorer 9 or higher to be installed.
	/// </summary>
	public class MsieJavascriptEngine : IJavascriptEngine
	{
		/// <summary>
		/// Internet Explorer engine
		/// </summary>
		private readonly MsieJsEngine _engine = new MsieJsEngine(useEcmaScript5Polyfill: true, useJson2Library: true);

		/// <summary>
		/// Loads an embedded JavaScript resource.
		/// </summary>
		/// <param name="resourceName">Name of the resource</param>
		public void LoadFromResource(string resourceName)
		{
			_engine.ExecuteResource(resourceName, Assembly.GetExecutingAssembly());
		}

		/// <summary>
		/// Executes the provided JavaScript code.
		/// </summary>
		/// <param name="code">JavaScript to execute</param>
		public void Execute(string code)
		{
			_engine.Execute(code);
		}

		/// <summary>
		/// Executes the provided JavaScript code, returning a result of the specified type.
		/// </summary>
		/// <typeparam name="T">Type to return</typeparam>
		/// <param name="code">Code to execute</param>
		/// <returns>Result of the JavaScript code</returns>
		public T Execute<T>(string code)
		{
			return _engine.Evaluate<T>(code);
		}

		/// <summary>
		/// Executes the provided JavaScript function, returning a result of the specified type.
		/// </summary>
		/// <typeparam name="T">Type to return</typeparam>
		/// <param name="function">Name of function to execute</param>
		/// <param name="args">Arguments to pass to function</param>
		/// <returns>Result of the JavaScript code</returns>
		public T ExecuteFunction<T>(string function, params object[] args)
		{
			return _engine.CallFunction<T>(function, args);
		}

		/// <summary>
		/// Sets a variable in the global scope.
		/// </summary>
		/// <typeparam name="T">Type of variable</typeparam>
		/// <param name="name">Name of the variable</param>
		/// <param name="value">Value of the variable</param>
		public void SetVariable<T>(string name, T value)
		{
			_engine.SetVariableValue(name, value);
		}
	}
}
