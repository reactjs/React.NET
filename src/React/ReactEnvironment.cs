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
using System.Threading;
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
		/// JavaScript variable set when user-provided scripts have been loaded
		/// </summary>
		private const string USER_SCRIPTS_LOADED_KEY = "_ReactNET_UserScripts_Loaded";
		/// <summary>
		/// Stack size to use for JSXTransformer if the default stack is insufficient
		/// </summary>
		private const int LARGE_STACK_SIZE = 2 * 1024 * 1024;

		/// <summary>
		/// Factory to create JavaScript engines
		/// </summary>
		private readonly IJavaScriptEngineFactory _engineFactory;
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
		/// Initializes a new instance of the <see cref="ReactEnvironment"/> class.
		/// </summary>
		/// <param name="engineFactory">The JavaScript engine factory</param>
		/// <param name="config">The site-wide configuration</param>
		/// <param name="cache">The cache to use for JSX compilation</param>
		/// <param name="fileSystem">File system wrapper</param>
		public ReactEnvironment(
			IJavaScriptEngineFactory engineFactory,
			IReactSiteConfiguration config,
			ICache cache,
			IFileSystem fileSystem
		)
		{
			_engineFactory = engineFactory;
			_config = config;
			_cache = cache;
			_fileSystem = fileSystem;
		}

		/// <summary>
		/// Gets the JavaScript engine for the current thread. If an engine has not yet been 
		/// created, create it and execute the startup scripts.
		/// </summary>
		private IJsEngine Engine
		{
			get
			{
				return _engineFactory.GetEngineForCurrentThread(InitialiseEngine);
			}
		}

		/// <summary>
		/// Loads standard React and JSXTransformer scripts into the engine.
		/// </summary>
		private void InitialiseEngine(IJsEngine engine)
		{
			// Load standard React scripts
			engine.Execute("var global = global || {};");
			// TODO: Handle errors "thrown" by console.error / console.warn?
			engine.Execute("var console = console || { log: function() {}, error: function() {}, warn: function() {} };");
			engine.ExecuteResource("React.Resources.react-0.9.0.js", GetType().Assembly);
			engine.ExecuteResource("React.Resources.JSXTransformer.js", GetType().Assembly);
			engine.Execute("var React = global.React");
		}

		/// <summary>
		/// Ensures any user-provided scripts have been loaded
		/// </summary>
		private void EnsureUserScriptsLoaded()
		{
			// Scripts already loaded into this environment, don't load them again
			if (Engine.HasVariable(USER_SCRIPTS_LOADED_KEY))
			{
				return;
			}

			foreach (var file in _config.Scripts)
			{
				var contents = LoadJsxFile(file);
				Execute(contents);
			}
			Engine.SetVariableValue(USER_SCRIPTS_LOADED_KEY, true);
		}

		/// <summary>
		/// Executes the provided JavaScript code.
		/// </summary>
		/// <param name="code">JavaScript to execute</param>
		public void Execute(string code)
		{
			Engine.Execute(code);
		}

		/// <summary>
		/// Executes the provided JavaScript code, returning a result of the specified type.
		/// </summary>
		/// <typeparam name="T">Type to return</typeparam>
		/// <param name="code">Code to execute</param>
		/// <returns>Result of the JavaScript code</returns>
		public T Execute<T>(string code)
		{
			return Engine.Evaluate<T>(code);
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
			EnsureUserScriptsLoaded();
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
			try
			{
				var encodedInput = JsonConvert.SerializeObject(input);
				var output = ExecuteWithLargerStackIfRequired<string>(string.Format(
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

		/// <summary>
		/// Attempts to execute the provided JavaScript code using the current engine. If an 
		/// exception is thrown, retries the execution using a new thread (and hence a new engine)
		/// with a larger maximum stack size.
		/// This is required because JSXTransformer uses a huge stack which ends up being larger 
		/// than what ASP.NET allows by default (256 KB).
		/// </summary>
		/// <typeparam name="T">Type to return from JavaScript call</typeparam>
		/// <param name="code">JavaScript code to execute</param>
		/// <returns>Result returned from JavaScript code</returns>
		private T ExecuteWithLargerStackIfRequired<T>(string code)
		{
			try
			{
				return Engine.Evaluate<T>(code);
			}
			catch (Exception)
			{
				// Assume the exception MAY be an "out of stack space" error. Try running the code 
				// in a different thread with larger stack. If the same exception occurs, we know
				// it wasn't a stack space issue.
				T result = default(T);
				Exception innerEx = null;
				var thread = new Thread(() =>
				{
					try
					{
						// New engine will be created here (as this is a new thread)
						result = Engine.Evaluate<T>(code);
						_engineFactory.DisposeEngineForCurrentThread();
					}
					catch (Exception threadEx)
					{
						// Unhandled exceptions in threads kill the whole process.
						// Pass the exception back to the parent thread to rethrow.
						innerEx = threadEx;
					}
				}, LARGE_STACK_SIZE);
				thread.Start();
				thread.Join();
				// Rethrow any exceptions that occured in the thread
				if (innerEx != null)
				{
					throw innerEx;
				}
				return result;
			}
		}
	}
}
