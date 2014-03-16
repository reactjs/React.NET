using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Jint;

namespace React
{
	/// <summary>
	/// Handles executing JavaScript using the Jint JavaScript engine.
	/// </summary>
	public class JintJavascriptEngine : IJavascriptEngine
	{
		/// <summary>
		/// Format string used for temporary variables used for function arguments
		/// </summary>
		private const string ARG_FORMAT = "_arg_{0}";
		/// <summary>
		/// Jint JavaScript engine
		/// </summary>
		private readonly Engine _engine;

		/// <summary>
		/// Initializes a new instance of the <see cref="JintJavascriptEngine"/> class.
		/// </summary>
		public JintJavascriptEngine()
		{
			_engine = new Engine();	
		}

		/// <summary>
		/// Loads an embedded JavaScript resource.
		/// </summary>
		/// <param name="resourceName">Name of the resource</param>
		public void LoadFromResource(string resourceName)
		{
			var code = GetResourceFile(resourceName);
			Execute(code);
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
			return (T)_engine.Execute(code).GetCompletionValue().ToObject();
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
			// TODO: Figure out if there's an easier way of doing this.
			// Jint doesn't seem to have a nice way to pass arguments to a function. So we use an
			// ugly hack here - Set all the arguments as global variables, and then pass these to 
			//the function.
			var argNames = new List<string>();
			var argIndex = 0;
			foreach (var arg in args)
			{
				var argName = string.Format(ARG_FORMAT, argIndex);
				argNames.Add(argName);
				SetVariable(argName, arg);
				argIndex++;
			}

			var functionCall = string.Format("{0}({1})", function, string.Join(", ", argNames));
			return Execute<T>(functionCall);
		}

		/// <summary>
		/// Sets a variable in the global scope.
		/// </summary>
		/// <typeparam name="T">Type of variable</typeparam>
		/// <param name="name">Name of the variable</param>
		/// <param name="value">Value of the variable</param>
		public void SetVariable<T>(string name, T value)
		{
			_engine.SetValue(name, value);
		}

		/// <summary>
		/// Loads the specified resource file
		/// </summary>
		/// <param name="resourceName">Name of the resource file</param>
		/// <returns>Contents of the resource file</returns>
		private static string GetResourceFile(string resourceName)
		{
			using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName))
			using (var reader = new StreamReader(stream))
			{
				return reader.ReadToEnd();
			}
		}
	}
}
