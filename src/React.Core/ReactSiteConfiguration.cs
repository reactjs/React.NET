/*
 *  Copyright (c) 2014-Present, Facebook, Inc.
 *  All rights reserved.
 *
 *  This source code is licensed under the BSD-style license found in the
 *  LICENSE file in the root directory of this source tree. An additional grant 
 *  of patent rights can be found in the PATENTS file in the same directory.
 */

using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace React
{
	/// <summary>
	/// Site-wide configuration for ReactJS.NET
	/// </summary>
	public class ReactSiteConfiguration : IReactSiteConfiguration
	{
		/// <summary>
		/// Gets or sets the site-side configuration
		/// </summary>
		public static IReactSiteConfiguration Configuration { get; set; }

		static ReactSiteConfiguration()
		{
			Configuration = new ReactSiteConfiguration();
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ReactSiteConfiguration"/> class.
		/// </summary>
		public ReactSiteConfiguration()
		{
			BabelConfig = new BabelConfig();
			ReuseJavaScriptEngines = true;
			AllowMsieEngine = true;
			LoadBabel = true;
			LoadReact = true;
			JsonSerializerSettings = new JsonSerializerSettings
			{
				StringEscapeHandling = StringEscapeHandling.EscapeHtml
			};
		}

		/// <summary>
		/// All the scripts that have been added to this configuration and require JSX 
		/// transformation to be run.
		/// </summary>
		private readonly IList<string> _scriptFiles = new List<string>();
		/// <summary>
		/// All the scripts that have been added to this configuration and do not require JSX
		/// transformation to be run.
		/// </summary>
		private readonly IList<string> _scriptFilesWithoutTransform = new List<string>();

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
		public IReactSiteConfiguration AddScript(string filename)
		{
			_scriptFiles.Add(filename);
			return this;
		}

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
		public IReactSiteConfiguration AddScriptWithoutTransform(string filename)
		{
			_scriptFilesWithoutTransform.Add(filename);
			return this;
		}

		/// <summary>
		/// Gets all the file paths that match the specified pattern. If the pattern is a plain
		/// path, just returns that path verbatim.
		/// </summary>
		/// <param name="glob">
		/// Patterns to search for (eg. <c>~/Scripts/*.js</c> or <c>~/Scripts/Awesome.js</c>
		/// </param>
		/// <returns>File paths that match this pattern</returns>
		private IEnumerable<string> Glob(string glob)
		{
			if (!glob.IsGlobPattern())
			{
				return new[] {glob};
			}
			// Directly touching the IoC container is not ideal, but we only want to pull the FileSystem
			// dependency if it's absolutely necessary.
			var fileSystem = AssemblyRegistration.Container.Resolve<IFileSystem>();
			return fileSystem.Glob(glob);
		}

		/// <summary>
		/// Gets a list of all the scripts that have been added to this configuration and require JSX
		/// transformation to be run.
		/// </summary>
		public IEnumerable<string> Scripts
		{
			// TODO: It's a bit strange to do the globbing here, ideally this class should just be a simple
			// bag of settings with no logic.
			get { return _scriptFiles.SelectMany(Glob); }
		}

		/// <summary>
		/// Gets a list of all the scripts that have been added to this configuration.
		/// </summary>
		public IEnumerable<string> ScriptsWithoutTransform
		{
			get { return _scriptFilesWithoutTransform.SelectMany(Glob); }
		}

		/// <summary>
		/// Gets or sets the configuration for JSON serializer.
		/// </summary>
		public JsonSerializerSettings JsonSerializerSettings { get; set; }

		/// <summary>
		/// Sets the configuration for json serializer.
		/// </summary>
		/// <param name="settings">Settings.</param>
		/// <remarks>
		/// Thic confiquration is used when component initialization script
		/// is being generated server-side.
		/// </remarks>
		public IReactSiteConfiguration SetJsonSerializerSettings(JsonSerializerSettings settings)
		{
			JsonSerializerSettings = settings;
			return this;
		}

		/// <summary>
		/// Gets or sets whether JavaScript engines should be reused across requests.
		/// </summary>
		public bool ReuseJavaScriptEngines { get; set; }
		/// <summary>
		/// Sets whether JavaScript engines should be reused across requests.
		/// </summary>
		public IReactSiteConfiguration SetReuseJavaScriptEngines(bool value)
		{
			ReuseJavaScriptEngines = value;
			return this;
		}

		/// <summary>
		/// Gets or sets the number of engines to initially start when a pool is created. 
		/// Defaults to <c>10</c>.
		/// </summary>
		public int? StartEngines { get; set; }
		/// <summary>
		/// Sets the number of engines to initially start when a pool is created. 
		/// Defaults to <c>10</c>.
		/// </summary>
		public IReactSiteConfiguration SetStartEngines(int? startEngines)
		{
			StartEngines = startEngines;
			return this;
		}

		/// <summary>
		/// Gets or sets the maximum number of engines that will be created in the pool. 
		/// Defaults to <c>25</c>.
		/// </summary>
		public int? MaxEngines { get; set; }
		/// <summary>
		/// Sets the maximum number of engines that will be created in the pool. 
		/// Defaults to <c>25</c>.
		/// </summary>
		public IReactSiteConfiguration SetMaxEngines(int? maxEngines)
		{
			MaxEngines = maxEngines;
			return this;
		}

		/// <summary>
		/// Gets or sets whether the MSIE engine should be used if V8 is unavailable.
		/// </summary>
		public bool AllowMsieEngine { get; set; }

		/// <summary>
		/// Sets whether the MSIE engine should be used if V8 is unavailable.
		/// </summary>
		/// <returns></returns>
		public IReactSiteConfiguration SetAllowMsieEngine(bool allowMsieEngine)
		{
			AllowMsieEngine = allowMsieEngine;
			return this;
		}

		/// <summary>
		/// Gets or sets whether the built-in version of React is loaded. If <c>false</c>, you must
		/// provide your own version of React.
		/// </summary>
		public bool LoadReact { get; set; }

		/// <summary>
		/// Sets whether the built-in version of React is loaded. If <c>false</c>, you must 
		/// provide your own version of React.
		/// </summary>
		/// <returns>The configuration, for chaining</returns>
		public IReactSiteConfiguration SetLoadReact(bool loadReact)
		{
			LoadReact = loadReact;
			return this;
		}

		/// <summary>
		/// Gets or sets whether Babel is loading. Disabling the loading of Babel can improve startup
		/// performance, but all your JSX files must be transformed beforehand (eg. through Babel,
		/// Webpack or Browserify).
		/// </summary>
		public bool LoadBabel { get; set; }

		/// <summary>
		/// Sets whether Babel is loading. Disabling the loading of Babel can improve startup
		/// performance, but all your JSX files must be transformed beforehand (eg. through Babel,
		/// Webpack or Browserify).
		/// </summary>
		public IReactSiteConfiguration SetLoadBabel(bool loadBabel)
		{
			LoadBabel = loadBabel;
			return this;
		}

		/// <summary>
		/// Gets or sets the Babel configuration to use.
		/// </summary>
		public BabelConfig BabelConfig { get; set; }

		/// <summary>
		/// Sets the Babel configuration to use.
		/// </summary>
		/// <returns>The configuration, for chaining</returns>
		public IReactSiteConfiguration SetBabelConfig(BabelConfig value)
		{
			BabelConfig = value;
			return this;
		}
	}
}
