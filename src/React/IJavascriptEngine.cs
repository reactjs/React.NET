namespace React
{
	/// <summary>
	/// Handles executing JavaScript.
	/// </summary>
	public interface IJavascriptEngine
	{
		/// <summary>
		/// Loads an embedded JavaScript resource.
		/// </summary>
		/// <param name="resourceName">Name of the resource</param>
		void LoadFromResource(string resourceName);

		/// <summary>
		/// Executes the provided JavaScript code.
		/// </summary>
		/// <param name="code">JavaScript to execute</param>
		void Execute(string code);

		/// <summary>
		/// Executes the provided JavaScript code, returning a result of the specified type.
		/// </summary>
		/// <typeparam name="T">Type to return</typeparam>
		/// <param name="code">Code to execute</param>
		/// <returns>Result of the JavaScript code</returns>
		T Execute<T>(string code);

		/// <summary>
		/// Executes the provided JavaScript function, returning a result of the specified type.
		/// </summary>
		/// <typeparam name="T">Type to return</typeparam>
		/// <param name="function">Name of function to execute</param>
		/// <param name="args">Arguments to pass to function</param>
		/// <returns>Result of the JavaScript code</returns>
		T ExecuteFunction<T>(string function, params object[] args);

		/// <summary>
		/// Sets a variable in the global scope.
		/// </summary>
		/// <typeparam name="T">Type of variable</typeparam>
		/// <param name="name">Name of the variable</param>
		/// <param name="value">Value of the variable</param>
		void SetVariable<T>(string name, T value);
	}
}
