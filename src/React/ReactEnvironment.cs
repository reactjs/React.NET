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
using System.Reflection;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading;
using JavaScriptEngineSwitcher.Core;

namespace React
{
	/// <summary>
	/// Request-specific ReactJS.NET environment. This is unique to the individual request and is 
	/// not shared.
	/// </summary>
	public class ReactEnvironment : IReactEnvironment
	{
		/// <summary>
		/// Format string used for React component container IDs
		/// </summary>
		private const string CONTAINER_ELEMENT_NAME = "react{0}";

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
		/// Hash algorithm for file-based cache
		/// </summary>
		private readonly IFileCacheHash _fileCacheHash;

		/// <summary>
		/// JSX Transformer instance for this environment
		/// </summary>
		private readonly Lazy<IJsxTransformer> _jsxTransformer;
		/// <summary>
		/// Version number of ReactJS.NET
		/// </summary>
		private readonly Lazy<string> _version = new Lazy<string>(GetVersion); 

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
		/// <param name="fileCacheHash">Hash algorithm for file-based cache</param>
		public ReactEnvironment(
			IJavaScriptEngineFactory engineFactory,
			IReactSiteConfiguration config,
			ICache cache,
			IFileSystem fileSystem,
			IFileCacheHash fileCacheHash
		)
		{
			_engineFactory = engineFactory;
			_config = config;
			_cache = cache;
			_fileSystem = fileSystem;
			_fileCacheHash = fileCacheHash;
			_jsxTransformer = new Lazy<IJsxTransformer>(() => 
				new JsxTransformer(this, _cache, _fileSystem, _fileCacheHash)
			);
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
		/// Gets the JSX Transformer for this environment.
		/// </summary>
		public IJsxTransformer JsxTransformer
		{
			get { return _jsxTransformer.Value; }
		}

		/// <summary>
		/// Determines if this JavaScript engine supports the JSX transformer.
		/// </summary>
		public bool EngineSupportsJsxTransformer
		{
			get { return Engine.SupportsJsxTransformer(); }
		}

		/// <summary>
		/// Gets the version number of ReactJS.NET
		/// </summary>
		public string Version
		{
			get { return _version.Value; }
		}

		/// <summary>
		/// Loads standard React and JSXTransformer scripts into the engine.
		/// </summary>
		private void InitialiseEngine(IJsEngine engine)
		{
			var thisAssembly = GetType().Assembly;
			engine.ExecuteResource("React.Resources.shims.js", thisAssembly);
			engine.ExecuteResource("React.Resources.react-with-addons.js", thisAssembly);
			engine.Execute("var React = global.React");

			// Only load JSX Transformer if engine supports it
			if (engine.SupportsJsxTransformer())
			{
				engine.ExecuteResource("React.Resources.JSXTransformer.js", thisAssembly);
			}
		}

		/// <summary>
		/// Ensures any user-provided scripts have been loaded
		/// </summary>
		private void EnsureUserScriptsLoaded()
		{
			// Scripts already loaded into this environment, don't load them again
			if (Engine.HasVariable(USER_SCRIPTS_LOADED_KEY) || _config == null)
			{
				return;
			}

			foreach (var file in _config.Scripts)
			{
				var contents = JsxTransformer.TransformJsxFile(file);
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
		/// Determines if the specified variable exists in the JavaScript engine
		/// </summary>
		/// <param name="name">Name of the variable</param>
		/// <returns><c>true</c> if the variable exists; <c>false</c> otherwise</returns>
		public bool HasVariable(string name)
		{
			return Engine.HasVariable(name);
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
		[Obsolete("Use JsxTransformer.TransformJsxFile")]
		public string LoadJsxFile(string filename)
		{
			return JsxTransformer.TransformJsxFile(filename);
		}

		/// <summary>
		/// Transforms JSX into regular JavaScript. The result is not cached. Use 
		/// <see cref="LoadJsxFile"/> if loading from a file since this will cache the result.
		/// </summary>
		/// <param name="input">JSX</param>
		/// <returns>JavaScript</returns>
		[Obsolete("Use JsxTransformer.TransformJsx")]
		public string TransformJsx(string input)
		{
			return JsxTransformer.TransformJsx(input);
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
		public T ExecuteWithLargerStackIfRequired<T>(string code)
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
					}
					catch (Exception threadEx)
					{
						// Unhandled exceptions in threads kill the whole process.
						// Pass the exception back to the parent thread to rethrow.
						innerEx = threadEx;
					}
					finally
					{
						_engineFactory.DisposeEngineForCurrentThread();
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

		/// <summary>
		/// Gets the ReactJS.NET version number. Use <see cref="Version" /> instead.
		/// </summary>
		private static string GetVersion()
		{
			var assembly = Assembly.GetExecutingAssembly();
			var rawVersion = FileVersionInfo.GetVersionInfo(assembly.Location).FileVersion;
			var lastDot = rawVersion.LastIndexOf('.');
			var version = rawVersion.Substring(0, lastDot);
			var build = rawVersion.Substring(lastDot + 1);
			return string.Format("{0} (build {1})", version, build);
		}
	}
}
