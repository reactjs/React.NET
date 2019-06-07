using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using JavaScriptEngineSwitcher.Core;
using JSPool;
using React.Exceptions;

namespace React
{
	/// <summary>
	/// Handles creation of JavaScript engines. All methods are thread-safe.
	/// </summary>
	public class JavaScriptEngineFactory : IDisposable, IJavaScriptEngineFactory
	{
		/// <summary>
		/// React configuration for the current site
		/// </summary>
		protected readonly IReactSiteConfiguration _config;
		/// <summary>
		/// Cache used for storing the pre-compiled scripts
		/// </summary>
		protected readonly ICache _cache;
		/// <summary>
		/// File system wrapper
		/// </summary>
		protected readonly IFileSystem _fileSystem;
		/// <summary>
		/// Function used to create new JavaScript engine instances.
		/// </summary>
		protected readonly Func<IJsEngine> _factory;
		/// <summary>
		/// The JavaScript Engine Switcher instance used by ReactJS.NET
		/// </summary>
		protected readonly IJsEngineSwitcher _jsEngineSwitcher;
		/// <summary>
		/// Contains all current JavaScript engine instances. One per thread, keyed on thread ID.
		/// </summary>
		protected readonly ConcurrentDictionary<int, IJsEngine> _engines
			= new ConcurrentDictionary<int, IJsEngine>();
		/// <summary>
		/// Pool of JavaScript engines to use
		/// </summary>
		protected IJsPool _pool;
		/// <summary>
		/// Whether this class has been disposed.
		/// </summary>
		protected bool _disposed;
		/// <summary>
		/// The exception that was thrown during the most recent recycle of the pool.
		/// </summary>
		protected Exception _scriptLoadException;

		/// <summary>
		/// Initializes a new instance of the <see cref="JavaScriptEngineFactory"/> class.
		/// </summary>
		public JavaScriptEngineFactory(
			IJsEngineSwitcher jsEngineSwitcher,
			IReactSiteConfiguration config,
			ICache cache,
			IFileSystem fileSystem
		)
		{
			_jsEngineSwitcher = jsEngineSwitcher;
			_config = config;
			_cache = cache;
			_fileSystem = fileSystem;
#pragma warning disable 618
			_factory = GetFactory(_jsEngineSwitcher);
#pragma warning restore 618
			if (_config.ReuseJavaScriptEngines)
			{
				_pool = CreatePool();
			}
		}

		/// <summary>
		/// Creates a new JavaScript engine pool.
		/// </summary>
		protected virtual IJsPool CreatePool()
		{
			var allFiles = _config.Scripts
				.Concat(_config.ScriptsWithoutTransform)
				.Select(_fileSystem.MapPath);

			var poolConfig = new JsPoolConfig
			{
				EngineFactory = _factory,
				Initializer = InitialiseEngine,
				WatchPath = _fileSystem.MapPath("~/"),
				WatchFiles = allFiles
			};
			if (_config.MaxEngines != null)
			{
				poolConfig.MaxEngines = _config.MaxEngines.Value;
			}
			if (_config.StartEngines != null)
			{
				poolConfig.StartEngines = _config.StartEngines.Value;
			}
			if (_config.MaxUsagesPerEngine != null)
			{
				poolConfig.MaxUsagesPerEngine = _config.MaxUsagesPerEngine.Value;
			}

			var pool = new JsPool(poolConfig);
			// Reset the recycle exception on recycle. If there *are* errors loading the scripts
			// during recycle, the errors will be caught in the initializer.
			pool.Recycled += (sender, args) => _scriptLoadException = null;
			return pool;
		}

		/// <summary>
		/// Loads standard React and Babel scripts into the engine.
		/// </summary>
		protected virtual void InitialiseEngine(IJsEngine engine)
		{
#if NET40
			var thisAssembly = typeof(ReactEnvironment).Assembly;
#else
			var thisAssembly = typeof(ReactEnvironment).GetTypeInfo().Assembly;
#endif
			LoadResource(engine, "React.Core.Resources.shims.js", thisAssembly);
			if (_config.LoadReact)
			{
				LoadResource(
					engine,
					_config.UseDebugReact
						? "React.Core.Resources.react.generated.js"
						: "React.Core.Resources.react.generated.min.js",
					thisAssembly
				);
			}

			LoadUserScripts(engine);
			if (!_config.LoadReact && _scriptLoadException == null)
			{
				// We expect the user to have loaded their own version of React in the scripts that
				// were loaded above, let's ensure that's the case. 
				EnsureReactLoaded(engine);
			}
		}

		/// <summary>
		/// Loads code from embedded JavaScript resource into the engine.
		/// </summary>
		/// <param name="engine">Engine to load a code from embedded JavaScript resource</param>
		/// <param name="resourceName">The case-sensitive resource name</param>
		/// <param name="assembly">The assembly, which contains the embedded resource</param>
		private void LoadResource(IJsEngine engine, string resourceName, Assembly assembly)
		{
			if (_config.AllowJavaScriptPrecompilation
				&& engine.TryExecuteResourceWithPrecompilation(_cache, resourceName, assembly))
			{
				// Do nothing.
			}
			else
			{
				engine.ExecuteResource(resourceName, assembly);
			}
		}

		/// <summary>
		/// Loads any user-provided scripts. Only scripts that don't need JSX transformation can
		/// run immediately here. JSX files are loaded in ReactEnvironment.
		/// </summary>
		/// <param name="engine">Engine to load scripts into</param>
		private void LoadUserScripts(IJsEngine engine)
		{
			foreach (var file in _config.ScriptsWithoutTransform)
			{
				try
				{
					if (_config.AllowJavaScriptPrecompilation
						&& engine.TryExecuteFileWithPrecompilation(_cache, _fileSystem, file))
					{
						// Do nothing.
					}
					else
					{
						engine.ExecuteFile(_fileSystem, file);
					}
				}
				catch (JsException ex)
				{
					// We can't simply rethrow the exception here, as it's possible this is running
					// on a background thread (ie. as a response to a file changing). If we did
					// throw the exception here, it would terminate the entire process. Instead,
					// save the exception, and then just rethrow it later when getting the engine.
					_scriptLoadException = new ReactScriptLoadException(string.Format(
						"Error while loading \"{0}\": {1}",
						file,
						ex.Message
					), ex);
				}
				catch (IOException ex)
				{
					_scriptLoadException = new ReactScriptLoadException(ex.Message, ex);
				}
			}
		}

		/// <summary>
		/// Ensures that React has been correctly loaded into the specified engine.
		/// </summary>
		/// <param name="engine">Engine to check</param>
		private static void EnsureReactLoaded(IJsEngine engine)
		{
			var globalsString = engine.CallFunction<string>("ReactNET_initReact");
			string[] globals = globalsString.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

			if (globals.Length != 0)
			{
				throw new ReactNotInitialisedException(
					$"React has not been loaded correctly: missing ({string.Join(", ", globals)})." +
					"Please expose your version of React as global variables named " +
					"'React', 'ReactDOM', and 'ReactDOMServer', or enable the 'LoadReact'" +
					"configuration option to use the built-in version of React."
				);
			}
		}

		/// <summary>
		/// Gets the JavaScript engine for the current thread. It is recommended to use
		/// <see cref="GetEngine"/> instead, which will pool/reuse engines.
		/// </summary>
		/// <returns>The JavaScript engine</returns>
		public virtual IJsEngine GetEngineForCurrentThread()
		{
			EnsureValidState();
			return _engines.GetOrAdd(Thread.CurrentThread.ManagedThreadId, id =>
			{
				var engine = _factory();
				InitialiseEngine(engine);
				EnsureValidState();
				return engine;
			});
		}

		/// <summary>
		/// Disposes the JavaScript engine for the current thread.
		/// </summary>
		public virtual void DisposeEngineForCurrentThread()
		{
			IJsEngine engine;
			if (_engines.TryRemove(Thread.CurrentThread.ManagedThreadId, out engine))
			{
				if (engine != null)
				{
					engine.Dispose();
				}
			}
		}

		/// <summary>
		/// Gets a JavaScript engine from the pool.
		/// </summary>
		/// <returns>The JavaScript engine</returns>
		public virtual PooledJsEngine GetEngine()
		{
			EnsureValidState();
			return _pool.GetEngine();
		}

		/// <summary>
		/// Gets a factory for the most appropriate JavaScript engine for the current environment.
		/// The first functioning JavaScript engine with the lowest priority will be used.
		/// </summary>
		/// <returns>Function to create JavaScript engine</returns>
		private static Func<IJsEngine> GetFactory(IJsEngineSwitcher jsEngineSwitcher)
		{
			string defaultEngineName = jsEngineSwitcher.DefaultEngineName;
			if (!string.IsNullOrWhiteSpace(defaultEngineName))
			{
				var engineFactory = jsEngineSwitcher.EngineFactories.Get(defaultEngineName);
				if (engineFactory != null)
				{
					return engineFactory.CreateEngine;
				}
				else
				{
					throw new ReactEngineNotFoundException(
						"Could not find a factory that creates an instance of the JavaScript " +
						"engine with name `" + defaultEngineName + "`.");
				}
			}

			if (jsEngineSwitcher.EngineFactories.Count == 0)
			{
				throw new ReactException("No JS engines were registered. Visit https://reactjs.net/docs for more information.");
			}

			var exceptionMessages = new List<string>();
			foreach (var engineFactory in jsEngineSwitcher.EngineFactories.GetRegisteredFactories())
			{
				IJsEngine engine = null;
				try
				{
					engine = engineFactory.CreateEngine();
					if (EngineIsUsable(engine))
					{
						// Success! Use this one.
						return engineFactory.CreateEngine;
					}
				}
				catch (JsEngineLoadException ex)
				{
					Trace.WriteLine(string.Format("Error initialising {0}: {1}", engineFactory, ex.Message));
					exceptionMessages.Add(ex.Message);
				}
				catch (Exception ex)
				{
					Trace.WriteLine(string.Format("Error initialising {0}: {1}", engineFactory, ex));
					exceptionMessages.Add(ex.ToString());
				}
				finally
				{
					if (engine != null)
					{
						engine.Dispose();
					}
				}
			}

			throw new ReactEngineNotFoundException("There was an error initializing the registered JS engines. " + string.Join(Environment.NewLine, exceptionMessages));
		}

		/// <summary>
		/// Performs a sanity check to ensure the specified engine type is usable.
		/// </summary>
		/// <param name="engine">Engine to test</param>
		/// <returns></returns>
		private static bool EngineIsUsable(IJsEngine engine)
		{
			// Perform a sanity test to ensure this engine is usable
			return engine.Evaluate<int>("1 + 1") == 2;
		}

		/// <summary>
		/// Clean up all engines
		/// </summary>
		public virtual void Dispose()
		{
			_disposed = true;
			foreach (var engine in _engines)
			{
				if (engine.Value != null)
				{
					engine.Value.Dispose();
				}
			}
			if (_pool != null)
			{
				_pool.Dispose();
				_pool = null;
			}
		}

		/// <summary>
		/// Ensures that this object has not been disposed, and that no error was thrown while
		/// loading the scripts.
		/// </summary>
		public void EnsureValidState()
		{
			if (_disposed)
			{
				throw new ObjectDisposedException(GetType().Name);
			}
			if (_scriptLoadException != null)
			{
				// This means an exception occurred while loading the script (eg. syntax error in the file)
				throw _scriptLoadException;
			}
		}
	}
}
