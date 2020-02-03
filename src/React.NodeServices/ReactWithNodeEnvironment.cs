/*
 * Copyright (c) Facebook, Inc. and its affiliates.
 *
 * This source code is licensed under the MIT license found in the
 * LICENSE file in the root directory of this source tree.
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using React.Exceptions;
using React.TinyIoC;

namespace React.NodeServices
{
	/// <summary>
	/// Request-specific ReactJS.NET environment. This is unique to the individual request and is
	/// not shared.
	/// </summary>
	public class ReactWithNodeEnvironment : IReactEnvironment, IDisposable
	{
		private Lazy<INodeJsEngine> _lazyEngine;
		/// <summary>
		/// JavaScript variable set when user-provided scripts have been loaded
		/// </summary>
		protected const string USER_SCRIPTS_LOADED_KEY = "_ReactNET_UserScripts_Loaded";
		/// <summary>
		/// Stack size to use for JSXTransformer if the default stack is insufficient
		/// </summary>
		protected const int LARGE_STACK_SIZE = 2 * 1024 * 1024;

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
		/// React Id generator
		/// </summary>
		private readonly IReactIdGenerator _reactIdGenerator;

		/// <summary>
		/// JSX Transformer instance for this environment
		/// </summary>
		protected readonly Lazy<IBabel> _babelTransformer;
		/// <summary>
		/// Version number of ReactJS.NET
		/// </summary>
		protected readonly Lazy<string> _version = new Lazy<string>(GetVersion);

		/// <summary>
		/// List of all components instantiated in this environment
		/// </summary>
		protected readonly IList<IReactComponent> _components = new List<IReactComponent>();

		/// <summary>
		/// Gets the <see cref="IReactEnvironment"/> for the current request. If no environment
		/// has been created for the current request, creates a new one.
		/// </summary>
		public static IReactEnvironment Current
		{
			get { return AssemblyRegistration.Container.Resolve<IReactEnvironment>(); }
		}

		/// <summary>
		/// Gets the <see cref="IReactEnvironment"/> for the current request. If no environment
		/// has been created for the current request, creates a new one.
		/// Also provides more specific error information in the event that ReactJS.NET is misconfigured.
		/// </summary>
		public static IReactEnvironment GetCurrentOrThrow
		{
			get
			{
				try
				{
					return Current;
				}
				catch (TinyIoCResolutionException ex)
				{
					throw new ReactNotInitialisedException(
#if NET451
						"ReactJS.NET has not been initialised correctly.",
#else
						"ReactJS.NET has not been initialised correctly. Please ensure you have " +
						"called services.AddReact() and app.UseReact() in your Startup.cs file.",
#endif
						ex
					);
				}
			}
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ReactEnvironment"/> class.
		/// </summary>
		/// <param name="engineFactory">The JavaScript engine factory</param>
		/// <param name="config">The site-wide configuration</param>
		/// <param name="cache">The cache to use for JSX compilation</param>
		/// <param name="fileSystem">File system wrapper</param>
		/// <param name="fileCacheHash">Hash algorithm for file-based cache</param>
		/// <param name="reactIdGenerator">React ID generator</param>
		public ReactWithNodeEnvironment(
			IReactSiteConfiguration config,
			ICache cache,
			IFileSystem fileSystem,
			IFileCacheHash fileCacheHash,
			IReactIdGenerator reactIdGenerator
		)
		{
			_config = config;
			_cache = cache;
			_fileSystem = fileSystem;
			_fileCacheHash = fileCacheHash;
			_reactIdGenerator = reactIdGenerator;
			_babelTransformer = new Lazy<IBabel>(() =>
				new Babel(this, _cache, _fileSystem, _fileCacheHash, _config)
			);
			_lazyEngine = new Lazy<INodeJsEngine>(() => 
			{
				var allFiles = _config.Scripts
					.Concat(_config.ScriptsWithoutTransform)
					.Select(_fileSystem.MapPath);

				var nodeInstance = _config.CreateNodeJsInstance();

				if (_config.LoadReact)
				{
					nodeInstance.ExecuteResource(_config.UseDebugReact
							? "React.Core.Resources.react.generated.js"
							: "React.Core.Resources.react.generated.min.js",
						typeof(ReactEnvironment).Assembly);
				}

				LoadUserScripts(nodeInstance);

				return nodeInstance;
			});
		}


		/// <summary>
		/// Loads any user-provided scripts. Only scripts that don't need JSX transformation can
		/// run immediately here. JSX files are loaded in ReactEnvironment.
		/// </summary>
		/// <param name="engine">Engine to load scripts into</param>
		private void LoadUserScripts(INodeJsEngine engine)
		{
			foreach (var file in _config.ScriptsWithoutTransform)
			{
				try
				{
					engine.ExecuteFile(_fileSystem, file);
				}
				catch (NodeJsException ex)
				{
				}
				catch (IOException ex)
				{
				}
			}
		}

		/// <summary>
		/// Gets the JavaScript engine to use for this environment.
		/// </summary>
		protected virtual INodeJsEngine Engine => _lazyEngine.Value;

		/// <summary>
		/// Gets the Babel transformer for this environment.
		/// </summary>
		public virtual IBabel Babel
		{
			get { return _babelTransformer.Value; }
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
		/// Ensures any user-provided scripts have been loaded. This only loads JSX files; files
		/// that need no transformation are loaded in JavaScriptEngineFactory.
		/// </summary>
		protected virtual void EnsureUserScriptsLoaded()
		{
			// Scripts already loaded into this environment, don't load them agai	n
			if (Engine.HasVariable(USER_SCRIPTS_LOADED_KEY) || _config == null)
			{
				return;
			}

			foreach (var file in _config.Scripts)
			{
				try
				{
					var contents = Babel.TransformFile(file);
					Engine.Execute(contents, file);
				}
				catch (NodeJsException ex)
				{
					throw new ReactScriptLoadException(string.Format(
						"Error while loading \"{0}\": {1}",
						file,
						ex.Message
					), ex);
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
			Engine.Execute(code);
		}

		/// <summary>
		/// Executes the provided JavaScript code, returning a result of the specified type.
		/// </summary>
		/// <typeparam name="T">Type to return</typeparam>
		/// <param name="code">Code to execute</param>
		/// <returns>Result of the JavaScript code</returns>
		public virtual T Execute<T>(string code)
		{
			return Engine.Evaluate<T>(code);
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
			return Engine.CallFunctionReturningJson<T>(function, args);
		}

		/// <summary>
		/// Determines if the specified variable exists in the JavaScript engine
		/// </summary>
		/// <param name="name">Name of the variable</param>
		/// <returns><c>true</c> if the variable exists; <c>false</c> otherwise</returns>
		public virtual bool HasVariable(string name)
		{
			return Engine.HasVariable(name);
		}

		/// <summary>
		/// Creates an instance of the specified React JavaScript component.
		/// </summary>
		/// <typeparam name="T">Type of the props</typeparam>
		/// <param name="componentName">Name of the component</param>
		/// <param name="props">Props to use</param>
		/// <param name="containerId">ID to use for the container HTML tag. Defaults to an auto-generated ID</param>
		/// <param name="clientOnly">True if server-side rendering will be bypassed. Defaults to false.</param>
		/// <param name="serverOnly">True if this component only should be rendered server-side. Defaults to false.</param>
		/// <returns>The component</returns>
		public virtual IReactComponent CreateComponent<T>(string componentName, T props, string containerId = null, bool clientOnly = false, bool serverOnly = false)
		{
			if (!clientOnly)
			{
				EnsureUserScriptsLoaded();
			}

			var component = new ReactComponent(this, _config, _reactIdGenerator, componentName, containerId)
			{
				ClientOnly = clientOnly,
				Props = props,
				ServerOnly = serverOnly
			};
			_components.Add(component);
			return component;
		}

		/// <summary>
		/// Adds the provided <see cref="IReactComponent"/> to the list of components to render client side.
		/// </summary>
		/// <param name="component">Component to add to client side render list</param>
		/// <param name="clientOnly">True if server-side rendering will be bypassed. Defaults to false.</param>
		/// <returns>The component</returns>
		public virtual IReactComponent CreateComponent(IReactComponent component, bool clientOnly = false)
		{
			if (!clientOnly)
			{
				EnsureUserScriptsLoaded();
			}

			_components.Add(component);
			return component;
		}

		/// <summary>
		/// Renders the JavaScript required to initialise all components client-side. This will
		/// attach event handlers to the server-rendered HTML.
		/// </summary>
		/// <param name="clientOnly">True if server-side rendering will be bypassed. Defaults to false.</param>
		/// <returns>JavaScript for all components</returns>
		public virtual string GetInitJavaScript(bool clientOnly = false)
		{
			using (var writer = new StringWriter())
			{
				GetInitJavaScript(writer, clientOnly);
				return writer.ToString();
			}
		}

		/// <summary>
		/// Renders the JavaScript required to initialise all components client-side. This will
		/// attach event handlers to the server-rendered HTML.
		/// </summary>
		/// <param name="writer">The <see cref="T:System.IO.TextWriter" /> to which the content is written</param>
		/// <param name="clientOnly">True if server-side rendering will be bypassed. Defaults to false.</param>
		/// <returns>JavaScript for all components</returns>
		public virtual void GetInitJavaScript(TextWriter writer, bool clientOnly = false)
		{
			foreach (var component in _components)
			{
				if (!component.ServerOnly)
				{
					component.RenderJavaScript(writer, waitForDOMContentLoad: false);
					writer.WriteLine(';');
				}
			}
		}

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
		public virtual T ExecuteWithBabel<T>(string function, params object[] args)
		{
			EnsureBabelLoaded(Engine);
			return Engine.CallFunctionReturningJson<T>(function, args);
		}

		/// <summary>
		/// Gets the ReactJS.NET version number. Use <see cref="Version" /> instead.
		/// </summary>
		private static string GetVersion()
		{
#if NET40
			var assembly = Assembly.GetExecutingAssembly();
			var rawVersion = FileVersionInfo.GetVersionInfo(assembly.Location).FileVersion;
#else
			var assembly = typeof(ReactEnvironment).GetTypeInfo().Assembly;
			var rawVersion = assembly.GetCustomAttribute<AssemblyFileVersionAttribute>().Version;
#endif
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
		}

		/// <summary>
		/// Returns the currently held JS engine to the pool. (no-op if engine pooling is disabled)
		/// </summary>
		public void ReturnEngineToPool()
		{
		}

		/// <summary>
		/// Gets the site-wide configuration.
		/// </summary>
		public virtual IReactSiteConfiguration Configuration
		{
			get { return _config; }
		}

		/// <summary>
		/// Ensures that Babel has been loaded into the JavaScript engine.
		/// </summary>
		private void EnsureBabelLoaded(INodeJsEngine engine)
		{
			// If Babel is disabled in the config, don't even try loading it
			if (!_config.LoadBabel)
			{
				throw new BabelNotLoadedException();
			}

			var babelLoaded = engine.Evaluate<bool>("typeof ReactNET_transform !== 'undefined'");
			if (!babelLoaded)
			{
#if NET40
				var assembly = typeof(ReactEnvironment).Assembly;
#else
				var assembly = typeof(ReactEnvironment).GetTypeInfo().Assembly;
#endif
				string resourceName = _config.BabelVersion == BabelVersions.Babel7 || _config.BabelVersion == null
					? "React.Core.Resources.babel.generated.min.js"
					: _config.BabelVersion == BabelVersions.Babel6
						? "React.Core.Resources.babel-legacy.generated.min.js"
						: throw new ReactConfigurationException("BabelVersion was not null, but did not contain a valid value.");

				engine.ExecuteResource(resourceName, assembly);
			}
		}
	}
}
