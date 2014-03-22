/*
 *  Copyright (c) 2014, Facebook, Inc.
 *  All rights reserved.
 *
 *  This source code is licensed under the BSD-style license found in the
 *  LICENSE file in the root directory of this source tree. An additional grant 
 *  of patent rights can be found in the PATENTS file in the same directory.
 */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using JavaScriptEngineSwitcher.Core;
using Newtonsoft.Json;
using React.Exceptions;

namespace React
{
	/// <summary>
	/// Request-specific React.NET environment. This is unique to the individual request and is 
	/// not shared.
	/// TODO: This is probably not thread safe at all (especially JSXTransformer)
	/// </summary>
	public class ReactEnvironment : IReactEnvironment
	{
		/// <summary>
		/// Format string used for React component container IDs
		/// </summary>
		private const string CONTAINER_ELEMENT_NAME = "react{0}";
		/// <summary>
		/// Cache key for JSX to JavaScript compilation
		/// </summary>
		private const string JSX_CACHE_KEY = "JSX_{0}";

		/// <summary>
		/// The JavaScript engine used in this environment
		/// </summary>
		private readonly IJsEngine _engine; 
		/// <summary>
		/// Site-wide configuration
		/// </summary>
		private readonly IReactSiteConfiguration _config;
		/// <summary>
		/// Cache used for storing compiled JSX
		/// </summary>
		private readonly ICache _cache;
		/// <summary>
		/// File system wrapper
		/// </summary>
		private readonly IFileSystem _fileSystem;

		/// <summary>
		/// Number of components instantiated in this environment
		/// </summary>
		private int _maxContainerId = 0;
		/// <summary>
		/// List of all components instantiated in this environment
		/// </summary>
		private readonly IList<IReactComponent> _components = new List<IReactComponent>();
		/// <summary>
		/// Whether the JSX Transformer has been loaded
		/// </summary>
		private bool _jsxTransformerLoaded = false;

		/// <summary>
		/// Initializes a new instance of the <see cref="ReactEnvironment"/> class.
		/// </summary>
		/// <param name="engine">The JavaScript engine</param>
		/// <param name="config">The site-wide configuration</param>
		/// <param name="cache">The cache to use for JSX compilation</param>
		/// <param name="fileSystem">File system wrapper</param>
		public ReactEnvironment(
			IJsEngine engine,
			IReactSiteConfiguration config,
			ICache cache,
			IFileSystem fileSystem
		)
		{
			// TODO: Scoping of engines. Engines can be reused if executed JavaScript has no 
			// side-effects. This will improve performance
			_engine = engine;
			_config = config;
			_cache = cache;
			_fileSystem = fileSystem;

			LoadStandardScripts();
			LoadExtraScripts();
		}

		/// <summary>
		/// Loads standard React and JSXTransformer scripts.
		/// </summary>
		private void LoadStandardScripts()
		{
			_engine.Execute("var global = global || {};");
			_engine.ExecuteResource("React.Resources.react-0.9.0.js", GetType().Assembly);
			_engine.Execute("var React = global.React");
		}

		/// <summary>
		/// Loads any user-supplied scripts from the configuration.
		/// </summary>
		private void LoadExtraScripts()
		{
			foreach (var file in _config.Scripts)
			{
				var contents = LoadJsxFile(file);
				Execute(contents);
			}
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
		/// Creates an instance of the specified React JavaScript component.
		/// </summary>
		/// <typeparam name="T">Type of the props</typeparam>
		/// <param name="componentName">Name of the component</param>
		/// <param name="props">Props to use</param>
		/// <returns>The component</returns>
		public IReactComponent CreateComponent<T>(string componentName, T props)
		{
			_maxContainerId++;
			var containerId = string.Format(CONTAINER_ELEMENT_NAME, _maxContainerId);
			var component = new ReactComponent(this, componentName, containerId)
			{
				Props = props
			};
			_components.Add(component);
			return component;
		}

		/// <summary>
		/// Renders the JavaScript required to initialise all components client-side. This will 
		/// attach event handlers to the server-rendered HTML.
		/// </summary>
		/// <returns>JavaScript for all components</returns>
		public string GetInitJavaScript()
		{
			var fullScript = new StringBuilder();
			foreach (var component in _components)
			{
				fullScript.Append(component.RenderJavaScript());
				fullScript.AppendLine(";");
			}
			return fullScript.ToString();
		}

		/// <summary>
		/// Loads a JSX file. Results of the JSX to JavaScript transformation are cached.
		/// </summary>
		/// <param name="filename">Name of the file to load</param>
		/// <returns>File contents</returns>
		public string LoadJsxFile(string filename)
		{
			var fullPath = _fileSystem.MapPath(filename);

			return _cache.GetOrInsert(
				key: string.Format(JSX_CACHE_KEY, filename),
				slidingExpiration: TimeSpan.FromMinutes(30),
				cacheDependencyFiles: new[] { fullPath },
				getData: () =>
				{
					Trace.WriteLine(string.Format("Parsing JSX from {0}", filename));

					var contents = _fileSystem.ReadAsString(filename);
					// Just return directly if there's no JSX annotation
					if (contents.Contains("@jsx"))
					{
						return TransformJsx(contents);
					}
					else
					{
						return contents;
					}

				}
			);
		}

		/// <summary>
		/// Transforms JSX into regular JavaScript. The result is not cached. Use 
		/// <see cref="LoadJsxFile"/> if loading from a file since this will cache the result.
		/// </summary>
		/// <param name="input">JSX</param>
		/// <returns>JavaScript</returns>
		public string TransformJsx(string input)
		{
			// Lazily load the JSX transformer JavaScript
			if (!_jsxTransformerLoaded)
			{
				_engine.ExecuteResource("React.Resources.JSXTransformer.js", GetType().Assembly);
				_jsxTransformerLoaded = true;
			}

			try
			{
				var encodedInput = JsonConvert.SerializeObject(input);
				var output = _engine.Evaluate<string>(string.Format(
					"global.JSXTransformer.transform({0}).code",
					encodedInput
				));
				return output;
			}
			catch (Exception ex)
			{
				throw new JsxException(ex.Message, ex);
			}
		}
	}
}
