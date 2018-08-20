/*
 *  Copyright (c) 2014-Present, Facebook, Inc.
 *  All rights reserved.
 *
 *  This source code is licensed under the BSD-style license found in the
 *  LICENSE file in the root directory of this source tree. An additional grant 
 *  of patent rights can be found in the PATENTS file in the same directory.
 */

using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace React
{
	/// <summary>
	/// Site-wide configuration for ReactJS.NET
	/// </summary>
	public interface IReactSiteConfiguration
	{
		/// <summary>
		/// Adds a script to the list of scripts that are executed. This should be called for all
		/// React components and their dependencies. If the script does not have any JSX in it
		/// (for example, it's built using Webpack or Gulp), use 
		/// <see cref="AddScriptWithoutTransform"/> instead.
		/// </summary>
		/// <param name="filename">
		/// Name of the file to execute. Should be a server relative path starting with ~ (eg. 
		/// <c>~/Scripts/Awesome.js</c>)
		/// </param>
		/// <returns>This configuration, for chaining</returns>
		IReactSiteConfiguration AddScript(string filename);

		/// <summary>
		/// Adds a script to the list of scripts that are executed. This is the same as
		/// <see cref="AddScript"/> except it does not run JSX transformation on the script and thus is
		/// more efficient.
		/// </summary>
		/// <param name="filename">
		/// Name of the file to execute. Should be a server relative path starting with ~ (eg. 
		/// <c>~/Scripts/Awesome.js</c>)
		/// </param>
		/// <returns>The configuration, for chaining</returns>
		IReactSiteConfiguration AddScriptWithoutTransform(string filename);

		/// <summary>
		/// Gets a list of all the scripts that have been added to this configuration and require JSX
		/// transformation to be run.
		/// </summary>
		IEnumerable<string> Scripts { get; }

		/// <summary>
		/// Gets a list of all the scripts that have been added to this configuration and do not 
		/// require JSX transformation to be run.
		/// </summary>
		IEnumerable<string> ScriptsWithoutTransform { get; } 

		/// <summary>
		/// Gets or sets whether JavaScript engines should be reused across requests.
		/// </summary>
		/// 
		bool ReuseJavaScriptEngines { get; set; }
		/// <summary>
		/// Sets whether JavaScript engines should be reused across requests.
		/// </summary>
		IReactSiteConfiguration SetReuseJavaScriptEngines(bool value);

		/// <summary>
		/// Gets or sets the configuration for JSON serializer.
		/// </summary>
		JsonSerializerSettings JsonSerializerSettings { get; set; }

		/// <summary>
		/// Sets the configuration for json serializer.
		/// </summary>
		/// <remarks>
		/// This confiquration is used when component initialization script
		/// is being generated server-side.
		/// </remarks>
		/// <param name="settings">The settings.</param>
		IReactSiteConfiguration SetJsonSerializerSettings(JsonSerializerSettings settings);

		/// <summary>
		/// Gets or sets the number of engines to initially start when a pool is created. 
		/// Defaults to <c>10</c>.
		/// </summary>
		int? StartEngines { get; set; }
		/// <summary>
		/// Sets the number of engines to initially start when a pool is created. 
		/// Defaults to <c>10</c>.
		/// </summary>
		IReactSiteConfiguration SetStartEngines(int? startEngines);

		/// <summary>
		/// Gets or sets the maximum number of engines that will be created in the pool. 
		/// Defaults to <c>25</c>.
		/// </summary>
		int? MaxEngines { get; set; }
		/// <summary>
		/// Sets the maximum number of engines that will be created in the pool. 
		/// Defaults to <c>25</c>.
		/// </summary>
		IReactSiteConfiguration SetMaxEngines(int? maxEngines);

		/// <summary>
		/// Gets or sets the maximum number of times an engine can be reused before it is disposed.
		/// <c>0</c> is unlimited. Defaults to <c>100</c>.
		/// </summary>
		int? MaxUsagesPerEngine { get; set; }
		/// <summary>
		/// Sets the maximum number of times an engine can be reused before it is disposed.
		/// <c>0</c> is unlimited. Defaults to <c>100</c>.
		/// </summary>
		IReactSiteConfiguration SetMaxUsagesPerEngine(int? maxUsagesPerEngine);

		/// <summary>
		/// Gets or sets whether the built-in version of React is loaded. If <c>false</c>, you must
		/// provide your own version of React.
		/// </summary>
		bool LoadReact { get; set; }
		/// <summary>
		/// Sets whether the built-in version of React is loaded. If <c>false</c>, you must 
		/// provide your own version of React.
		/// </summary>
		/// <returns>The configuration, for chaining</returns>
		IReactSiteConfiguration SetLoadReact(bool loadReact);

		/// <summary>
		/// Gets or sets whether Babel is loading. Disabling the loading of Babel can improve startup
		/// performance, but all your JSX files must be transformed beforehand (eg. through Babel,
		/// Webpack or Browserify).
		/// </summary>
		bool LoadBabel { get; set; }
		/// <summary>
		/// Sets whether Babel is loading. Disabling the loading of Babel can improve startup
		/// performance, but all your JSX files must be transformed beforehand (eg. through Babel,
		/// Webpack or Browserify).
		/// </summary>
		IReactSiteConfiguration SetLoadBabel(bool loadBabel);

		/// <summary>
		/// Gets or sets the Babel configuration to use.
		/// </summary>
		BabelConfig BabelConfig { get; set; }
		/// <summary>
		/// Sets the Babel configuration to use.
		/// </summary>
		/// <returns>The configuration, for chaining</returns>
		IReactSiteConfiguration SetBabelConfig(BabelConfig value);

		/// <summary>
		/// Gets or sets whether to use the debug version of React. This is slower, but gives
		/// useful debugging tips.
		/// </summary>
		bool UseDebugReact { get; set; }
		/// <summary>
		/// Sets whether to use the debug version of React. This is slower, but gives
		/// useful debugging tips.
		/// </summary>
		IReactSiteConfiguration SetUseDebugReact(bool value);

		/// <summary>
		/// Gets or sets whether server-side rendering is enabled.
		/// </summary>
		bool UseServerSideRendering { get; set; }
		/// <summary>
		/// Disables server-side rendering. This is useful when debugging your scripts.
		/// </summary>
		IReactSiteConfiguration DisableServerSideRendering();

		/// <summary>
		/// An exception handler which will be called if a render exception is thrown.
		/// If unset, unhandled exceptions will be thrown for all component renders.
		/// </summary>
		Action<Exception, string, string> ExceptionHandler { get; set; }

		/// <summary>
		/// Sets an exception handler which will be called if a render exception is thrown.
		/// If unset, unhandled exceptions will be thrown for all component renders.
		/// </summary>
		/// <param name="handler"></param>
		/// <returns></returns>
		IReactSiteConfiguration SetExceptionHandler(Action<Exception, string, string> handler);

		/// <summary>
		/// A provider that returns a nonce to be used on any script tags on the page. 
		/// This value must match the nonce used in the Content Security Policy header on the response.
		/// </summary>
		Func<string> ScriptNonceProvider { get; set; }

		/// <summary>
		/// Sets a provider that returns a nonce to be used on any script tags on the page. 
		/// This value must match the nonce used in the Content Security Policy header on the response.
		/// </summary>
		/// <param name="provider"></param>
		/// <returns></returns>
		IReactSiteConfiguration SetScriptNonceProvider(Func<string> provider);
	}
}
