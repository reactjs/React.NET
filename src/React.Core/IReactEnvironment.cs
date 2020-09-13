/*
 * Copyright (c) Facebook, Inc. and its affiliates.
 *
 * This source code is licensed under the MIT license found in the
 * LICENSE file in the root directory of this source tree.
 */


using System.Collections.Generic;
using System.IO;

namespace React
{
	/// <summary>
	/// Request-specific ReactJS.NET environment. This is unique to the individual request and is
	/// not shared.
	/// </summary>
	public interface IReactEnvironment
	{
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
		/// Attempts to execute the provided JavaScript code using a non-pooled JavaScript engine (ie.
		/// creates a new JS engine per-thread). This is because Babel uses a LOT of memory, so we
		/// should completely dispose any engines that have loaded Babel in order to conserve memory.
		///
		/// If an exception is thrown, retries the execution using a new thread (and hence a new engine)
		/// with a larger maximum stack size.
		/// This is required because JSXTransformer uses a huge stack which ends up being larger
		/// than what ASP.NET allows by default (256 KB).
		/// </summary>
		/// <typeparam name="T">Type to return from JavaScript call</typeparam>
		/// <param name="function">JavaScript function to execute</param>
		/// <param name="args">Arguments to pass to function</param>
		/// <returns>Result returned from JavaScript code</returns>
		T ExecuteWithBabel<T>(string function, params object[] args);

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
		/// <param name="clientOnly">True if server-side rendering will be bypassed. Defaults to false.</param>
		/// <param name="serverOnly">True if this component only should be rendered server-side. Defaults to false.</param>
		/// <param name="skipLazyInit">Skip adding to components list, which is used during GetInitJavascript</param>
		/// <returns>The component</returns>
		IReactComponent CreateComponent<T>(string componentName, T props, string containerId = null, bool clientOnly = false, bool serverOnly = false, bool skipLazyInit = false);

		/// <summary>
		/// Adds the provided <see cref="IReactComponent"/> to the list of components to render client side.
		/// </summary>
		/// <param name="component">Component to add to client side render list</param>
		/// <param name="clientOnly">True if server-side rendering will be bypassed. Defaults to false.</param>
		/// <returns>The component</returns>
		IReactComponent CreateComponent(IReactComponent component, bool clientOnly = false);

		/// <summary>
		/// Renders the JavaScript required to initialise all components client-side. This will
		/// attach event handlers to the server-rendered HTML.
		/// </summary>
		/// <param name="clientOnly">True if server-side rendering will be bypassed. Defaults to false.</param>
		/// <returns>JavaScript for all components</returns>
		string GetInitJavaScript(bool clientOnly = false);

		/// <summary>
		/// Gets the JSX Transformer for this environment.
		/// </summary>
		IBabel Babel { get; }

		/// <summary>
		/// Returns the currently held JS engine to the pool. (no-op if engine pooling is disabled)
		/// </summary>
		void ReturnEngineToPool();

		/// <summary>
		/// Gets the site-wide configuration.
		/// </summary>
		IReactSiteConfiguration Configuration { get; }

		/// <summary>
		/// Renders the JavaScript required to initialise all components client-side. This will
		/// attach event handlers to the server-rendered HTML.
		/// </summary>
		/// <param name="writer">The <see cref="T:System.IO.TextWriter" /> to which the content is written</param>
		/// <param name="clientOnly">True if server-side rendering will be bypassed. Defaults to false.</param>
		/// <returns>JavaScript for all components</returns>
		void GetInitJavaScript(TextWriter writer, bool clientOnly = false);

		/// <summary>
		/// Returns a list of paths to scripts generated by the React app
		/// </summary>
		IEnumerable<string> GetScriptPaths();

		/// <summary>
		/// Returns a list of paths to stylesheets generated by the React app
		/// </summary>
		IEnumerable<string> GetStylePaths();
	}
}
