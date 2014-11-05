/*
 *  Copyright (c) 2014, Facebook, Inc.
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
		/// React components and their dependencies.
		/// </summary>
		/// <param name="filename">
		/// Name of the file to execute. Should be a server relative path starting with ~ (eg. 
		/// <c>~/Scripts/Awesome.js</c>)
		/// </param>
		/// <returns>This configuration, for chaining</returns>
		IReactSiteConfiguration AddScript(string filename);

		/// <summary>
		/// Gets a list of all the scripts that have been added to this configuration.
		/// </summary>
		IList<string> Scripts { get; }

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
		/// Gets the configuration for json serializer.
		/// </summary>
		JsonSerializerSettings JsonSerializerSettings { get; }

		/// <summary>
		/// Sets the configuration for json serializer.
		/// </summary>
		/// <remarks>
		/// Thic confiquration is used when component initialization script
		/// is being generated server-side.
		/// </remarks>
		/// <param name="settings">The settings.</param>
		IReactSiteConfiguration SetJsonSerializerSettings(JsonSerializerSettings settings);
	}
}
