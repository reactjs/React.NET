﻿/*
 *  Copyright (c) 2014-2015, Facebook, Inc.
 *  All rights reserved.
 *
 *  This source code is licensed under the BSD-style license found in the
 *  LICENSE file in the root directory of this source tree. An additional grant 
 *  of patent rights can be found in the PATENTS file in the same directory.
 */

using Newtonsoft.Json;
using System.Collections.Generic;

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
		/// A value indicating if es6 syntax should be rewritten.
		/// </summary>
		/// <returns><c>true</c> if support for es6 syntax should be rewritten.</returns>
		bool UseHarmony { get; set; }

		/// <summary>
		/// Specifies whether ES6 (harmony) syntax should be transformed
		/// </summary>
		IReactSiteConfiguration SetUseHarmony(bool useHarmony);

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
		/// Gets or sets whether Flow types should be stripped out.
		/// </summary>
		bool StripTypes { get; set; }
		/// <summary>
		/// Sets whether Flow types should be stripped out.
		/// </summary>
		IReactSiteConfiguration SetStripTypes(bool stripTypes);

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
		/// Gets or sets whether the MSIE engine should be used if V8 is unavailable.
		/// </summary>
		bool AllowMsieEngine { get; set; }
		/// <summary>
		/// Sets whether the MSIE engine should be used if V8 is unavailable.
		/// </summary>
		/// <returns></returns>
		IReactSiteConfiguration SetAllowMsieEngine(bool allowMsieEngine);

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
		/// Gets or sets whether or not the JSX transforms should be loaded into the JavaScript engine. This setting is only
		/// applicable when <see cref="LoadReact"></see> is true.
		/// </summary>
		bool LoadJsxTransform { get; set; }

		/// <summary>
		/// Sets whether the JSX Transforms should be loaded into the Javscript engine. This setting is only
		/// applicable when <see cref="LoadReact"></see> is true.
		/// </summary>
		/// <returns>The configuration, for chaining</returns>
		IReactSiteConfiguration SetLoadJsxTransform(bool loadJsxTransform);
	}
}
