/*
 *  Copyright (c) 2014-2015, Facebook, Inc.
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
using System.Text;
using System.Threading;
using JavaScriptEngineSwitcher.Core;
using JavaScriptEngineSwitcher.Core.Helpers;
using Newtonsoft.Json;
using React.Exceptions;

namespace React
{
	/// <summary>
	/// Request-specific ReactJS.NET environment. This is unique to the individual request and is 
	/// not shared.
	/// </summary>
	public class ReactEnvironment : IReactEnvironment, IDisposable
	{
		/// <summary>
		/// Format string used for React component container IDs
		/// </summary>
		protected const string CONTAINER_ELEMENT_NAME = "react{0}";

		/// <summary>
		/// JavaScript variable set when user-provided scripts have been loaded
		/// </summary>
		protected const string USER_SCRIPTS_LOADED_KEY = "_ReactNET_UserScripts_Loaded";
		/// <summary>
		/// Stack size to use for JSXTransformer if the default stack is insufficient
		/// </summary>
		protected const int LARGE_STACK_SIZE = 2 * 1024 * 1024;

		/// <summary>
		/// Factory to create JavaScript engines
		/// </summary>
		protected readonly IJavaScriptEngineFactory _engineFactory;
		/// <summary>
		/// Site-wide configuration
		/// </summary>
		protected readonly IReactSiteConfiguration _config;
		/// <summary>
		/// Cache used for storing compiled JSX
		/// </summary>
		protected readonly ICache _cache;
		/// <summary>
		/// File system wrapper
		/// </summary>
		protected readonly IFileSystem _fileSystem;
		/// <summary>
		/// Hash algorithm for file-based cache
		/// </summary>
		protected readonly IFileCacheHash _fileCacheHash;

		/// <summary>
		/// JSX Transformer instance for this environment
		/// </summary>
		protected readonly Lazy<IJsxTransformer> _jsxTransformer;
		/// <summary>
		/// Version number of ReactJS.NET
		/// </summary>
		protected readonly Lazy<string> _version = new Lazy<string>(GetVersion);
		/// <summary>
		/// Contains an engine acquired from a pool of engines. Only used if 
		/// <see cref="IReactSiteConfiguration.ReuseJavaScriptEngines"/> is enabled.
		/// </summary>
		protected readonly Lazy<IJsEngine> _engineFromPool;

		/// <summary>
		/// Number of components instantiated in this environment
		/// </summary>
		protected int _maxContainerId = 0;
		/// <summary>
		/// List of all components instantiated in this environment
		/// </summary>
		protected readonly IList<IReactComponent> _components = new List<IReactComponent>();

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
				new JsxTransformer(this, _cache, _fileSystem, _fileCacheHash, _config)
			);
			_engineFromPool = new Lazy<IJsEngine>(() => _engineFactory.GetEngine());
		}

		/// <summary>
		/// Gets the JavaScript engine to use for this environment.
		/// </summary>
		protected virtual IJsEngine Engine
		{
			get
			{
				return _config.ReuseJavaScriptEngines
					? _engineFromPool.Value
					: _engineFactory.GetEngineForCurrentThread();
			}
		}

		/// <summary>
		/// Gets the JSX Transformer for this environment.
		/// </summary>
		public virtual IJsxTransformer JsxTransformer
		{
			get { return _jsxTransformer.Value; }
		}

		/// <summary>
		/// Determines if this JavaScript engine supports the JSX transformer.
		/// </summary>
		public virtual bool EngineSupportsJsxTransformer
		{
			get { return Engine.SupportsJsxTransformer(); }
		}

		/// <summary>
		/// Gets the version of the JavaScript engine in use by ReactJS.NET
		/// </summary>
		public virtual string EngineVersion
		{
			get { return Engine.Name + " " + Engine.Version; }
		}

		/// <summary>
		/// Gets the version number of ReactJS.NET
		/// </summary>
		public virtual string Version
		{
			get { return _version.Value; }
		}

		/// <summary>
		/// Ensures any user-provided scripts have been loaded
		/// </summary>
		protected virtual void EnsureUserScriptsLoaded()
		{
			// Scripts already loaded into this environment, don't load them again
			if (Engine.HasVariable(USER_SCRIPTS_LOADED_KEY) || _config == null)
			{
				return;
			}

			foreach (var file in _config.Scripts)
			{
				var contents = JsxTransformer.TransformJsxFile(file);
				try
				{
					Execute(contents);
				}
				catch (JsRuntimeException ex)
				{
					throw new ReactScriptLoadException(string.Format(
						"Error while loading \"{0}\": {1}",
						file,
						ex.Message
					));
				}
			}
			Engine.SetVariableValue(USER_SCRIPTS_LOADED_KEY, true);
		}

		/// <summary>
		/// Executes the provided JavaScript code.
		/// </summary>
		/// <param name="code">JavaScript to execute</param>
		public virtual void Execute(string code)
		{
			try
			{
				Engine.Execute(code);
			}
			catch (JsRuntimeException ex)
			{
				throw WrapJavaScriptRuntimeException(ex);
			}
		}

		/// <summary>
		/// Executes the provided JavaScript code, returning a result of the specified type.
		/// </summary>
		/// <typeparam name="T">Type to return</typeparam>
		/// <param name="code">Code to execute</param>
		/// <returns>Result of the JavaScript code</returns>
		public virtual T Execute<T>(string code)
		{
			try
			{
				return Engine.Evaluate<T>(code);
			}
			catch (JsRuntimeException ex)
			{
				throw WrapJavaScriptRuntimeException(ex);
			}
		}

		/// <summary>
		/// Executes the provided JavaScript function, returning a result of the specified type.
		/// </summary>
		/// <typeparam name="T">Type to return</typeparam>
		/// <param name="function">JavaScript function to execute</param>
		/// <param name="args">Arguments to pass to function</param>
		/// <returns>Result of the JavaScript code</returns>
		public virtual T Execute<T>(string function, params object[] args)
		{
			try
			{
				if (ValidationHelpers.IsSupportedType(typeof (T)))
				{
					// Type is supported directly (ie. a scalar type like string/int/bool)
					// Just execute the function directly.
					return Engine.CallFunction<T>(function, args);
				}
				// The type is not a scalar type. Assume the function will return its result as
				// JSON.
				var resultJson = Engine.CallFunction<string>(function, args);
				try
				{
					return JsonConvert.DeserializeObject<T>(resultJson);
				}
				catch (JsonReaderException ex)
				{
					throw new ReactException(string.Format(
						"{0} did not return valid JSON: {1}.\n\n{2}",
						function, ex.Message, resultJson
					));
				}
			}
			catch (JsRuntimeException ex)
			{
				throw WrapJavaScriptRuntimeException(ex);
			}
		}

		/// <summary>
		/// Determines if the specified variable exists in the JavaScript engine
		/// </summary>
		/// <param name="name">Name of the variable</param>
		/// <returns><c>true</c> if the variable exists; <c>false</c> otherwise</returns>
		public virtual bool HasVariable(string name)
		{
			try
			{
				return Engine.HasVariable(name);
			}
			catch (JsRuntimeException ex)
			{
				throw WrapJavaScriptRuntimeException(ex);
			}
		}

		/// <summary>
		/// Creates an instance of the specified React JavaScript component.
		/// </summary>
		/// <typeparam name="T">Type of the props</typeparam>
		/// <param name="componentName">Name of the component</param>
		/// <param name="props">Props to use</param>
		/// <param name="containerId">ID to use for the container HTML tag. Defaults to an auto-generated ID</param>
		/// <returns>The component</returns>
		public virtual IReactComponent CreateComponent<T>(string componentName, T props, string containerId = null)
		{
			EnsureUserScriptsLoaded();
			if (string.IsNullOrEmpty(containerId))
			{
				_maxContainerId++;
				containerId = string.Format(CONTAINER_ELEMENT_NAME, _maxContainerId);	
			}
			
			var component = new ReactComponent(this, _config, componentName, containerId)
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
		public virtual string GetInitJavaScript()
		{
			var fullScript = new StringBuilder();
			foreach (var component in _components)
			{
				fullScript.Append(component.RenderJavaScript());
				fullScript.AppendLine(";");
			}
			
			// Also propagate any server-side console.log calls to corresponding client-side calls.
			var consoleCalls = Execute<string>("console.getCalls()");
			fullScript.Append(consoleCalls);

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
		/// <param name="function">JavaScript function to execute</param>
		/// <param name="args">Arguments to pass to function</param>
		/// <returns>Result returned from JavaScript code</returns>
		public virtual T ExecuteWithLargerStackIfRequired<T>(string function, params object[] args)
		{
			// This hack is not required when pooling JavaScript engines, since pooled MSIE
			// engines already execute on their own thread with a larger stack.
			if (_config.ReuseJavaScriptEngines)
			{
				return Execute<T>(function, args);
			}

			try
			{
				return Execute<T>(function, args);
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
					// New engine will be created here (as this is a new thread)
					var engine = _engineFactory.GetEngineForCurrentThread();
					try
					{
						result = engine.CallFunction<T>(function, args);
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

		/// <summary>
		/// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
		/// </summary>
		public virtual void Dispose()
		{
			_engineFactory.DisposeEngineForCurrentThread();
			if (_engineFromPool.IsValueCreated)
			{
				_engineFactory.ReturnEngineToPool(_engineFromPool.Value);
			}
		}

		/// <summary>
		/// Updates the Message of a <see cref="JsRuntimeException"/> to be more useful, containing
		/// the line and column numbers.
		/// </summary>
		/// <param name="ex">Original exception</param>
		/// <returns>New exception</returns>
		protected virtual JsRuntimeException WrapJavaScriptRuntimeException(JsRuntimeException ex)
		{
			return new JsRuntimeException(string.Format(
				"{0}\r\nLine: {1}\r\nColumn:{2}",
				ex.Message,
				ex.LineNumber,
				ex.ColumnNumber
			), ex.EngineName, ex.EngineVersion)
			{
				ErrorCode = ex.ErrorCode,
				Category = ex.Category,
				LineNumber = ex.LineNumber,
				ColumnNumber = ex.ColumnNumber,
				SourceFragment = ex.SourceFragment,
				Source = ex.Source,
			};
		}

		/// <summary>
		/// Gets the site-wide configuration.
		/// </summary>
		public virtual IReactSiteConfiguration Configuration
		{
			get { return _config; }
		}
	}
}
