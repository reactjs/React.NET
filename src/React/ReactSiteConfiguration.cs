/*
 *  Copyright (c) 2014, Facebook, Inc.
 *  All rights reserved.
 *
 *  This source code is licensed under the BSD-style license found in the
 *  LICENSE file in the root directory of this source tree. An additional grant 
 *  of patent rights can be found in the PATENTS file in the same directory.
 */

using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace React
{
	/// <summary>
	/// Site-wide configuration for ReactJS.NET
	/// </summary>
	public class ReactSiteConfiguration : IReactSiteConfiguration
	{
		private static IReactSiteConfiguration _instance = new ReactSiteConfiguration();

		/// <summary>
		/// All the scripts that have been added to this configuration
		/// </summary>
		private readonly IList<string> _scriptFiles = new List<string>();

		/// <summary>
		/// Gets or sets the site-side configuration
		/// </summary>
		public static IReactSiteConfiguration Configuration
		{
			get
			{
				return _instance;
			}
		}

		/// <summary>
		/// Adds a script to the list of scripts that are executed. This should be called for all
		/// React components and their dependencies.
		/// </summary>
		/// <param name="filename">
		/// Name of the file to execute. Should be a server relative path starting with ~ (eg. 
		/// <c>~/Scripts/Awesome.js</c>)
		/// </param>
		/// <returns>This configuration, for chaining</returns>
		public IReactSiteConfiguration AddScript(string filename)
		{
			_scriptFiles.Add(filename);
			return this;
		}

		/// <summary>
		/// Gets a list of all the scripts that have been added to this configuration.
		/// </summary>
		public IList<string> Scripts
		{
			get { return new ReadOnlyCollection<string>(_scriptFiles); }
		}

		/// <summary>
		/// A value indicating if es6 syntax should be rewritten.
		/// </summary>
		/// <returns><c>true</c> if support for es6 syntax should be rewritten.</returns>
		public bool UseHarmony { get; set; }
	}
}
