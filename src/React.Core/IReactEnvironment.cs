/*
 *  Copyright (c) 2014-2015, Facebook, Inc.
 *  All rights reserved.
 *
 *  This source code is licensed under the BSD-style license found in the
 *  LICENSE file in the root directory of this source tree. An additional grant 
 *  of patent rights can be found in the PATENTS file in the same directory.
 */

using System;

namespace React
{
	/// <summary>
	/// Request-specific ReactJS.NET environment. This is unique to the individual request and is 
	/// not shared.
	/// </summary>
	public interface IReactEnvironment
	{
		/// <summary>
		/// Determines if this JavaScript engine supports the JSX transformer.
		/// </summary>
		bool EngineSupportsJsxTransformer { get; }

		/// <summary>
		/// Gets the version number of ReactJS.NET
		/// </summary>
		string Version { get; }

		/// <summary>
		/// Gets the name and version of the JavaScript engine in use by ReactJS.NET
		/// </summary>
		string EngineVersion { get; }

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
		/// <param name="function">JavaScript function to execute</param>
		/// <param name="args">Arguments to pass to function</param>
		/// <returns>Result of the JavaScript code</returns>
		T Execute<T>(string function, params object[] args);

		/// <summary>
		/// Attempts to execute the provided JavaScript code using the current engine. If an 
		/// exception is thrown, retries the execution using a new thread (and hence a new engine)
		/// with a larger maximum stack size.
		/// This is required because JSXTransformer uses a huge stack which ends up being larger 
		/// than what ASP.NET allows by default (256 KB).
		/// </summary>
		/// <typeparam name="T">Type to return from JavaScript call</typeparam>
		/// <param name="function">JavaScript function to execute</param>
		/// <param name="args">Arguments to pass to function</param>
		/// <returns>Result returned from JavaScript code</returns>
		T ExecuteWithLargerStackIfRequired<T>(string function, params object[] args);

		/// <summary>
		/// Determines if the specified variable exists in the JavaScript engine
		/// </summary>
		/// <param name="name">Name of the variable</param>
		/// <returns><c>true</c> if the variable exists; <c>false</c> otherwise</returns>
		bool HasVariable(string name);

		/// <summary>
		/// Creates an instance of the specified React JavaScript component.
		/// </summary>
		/// <typeparam name="T">Type of the props</typeparam>
		/// <param name="componentName">Name of the component</param>
		/// <param name="props">Props to use</param>
		/// <param name="containerId">ID to use for the container HTML tag. Defaults to an auto-generated ID</param>
		/// <returns>The component</returns>
		IReactComponent CreateComponent<T>(string componentName, T props, string containerId = null);

		/// <summary>
		/// Renders the JavaScript required to initialise all components client-side. This will 
		/// attach event handlers to the server-rendered HTML.
		/// </summary>
		/// <returns>JavaScript for all components</returns>
		string GetInitJavaScript();

		/// <summary>
		/// Gets the JSX Transformer for this environment.
		/// </summary>
		IJsxTransformer JsxTransformer { get; }
	}
}
