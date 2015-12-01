using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using JavaScriptEngineSwitcher.Core;
using JavaScriptEngineSwitcher.Msie;
using JavaScriptEngineSwitcher.V8;
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
		/// File system wrapper
		/// </summary>
		protected readonly IFileSystem _fileSystem;
		/// <summary>
		/// Function used to create new JavaScript engine instances.
		/// </summary>
		protected readonly Func<IJsEngine> _factory; 
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
			IEnumerable<Registration> availableFactories, 
			IReactSiteConfiguration config,
			IFileSystem fileSystem
		)
		{
			_config = config;
			_fileSystem = fileSystem;
			_factory = GetFactory(availableFactories, config.AllowMsieEngine);
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
			var thisAssembly = typeof(ReactEnvironment).Assembly;
			engine.ExecuteResource("React.Resources.shims.js", thisAssembly);
			if (_config.LoadReact)
			{
				// TODO: Add option to choose whether to load dev vs prod version of React.
				engine.ExecuteResource("React.Resources.react.generated.js", thisAssembly);
			}

			LoadUserScripts(engine);
			if (!_config.LoadReact)
			{
				// We expect to user to have loaded their own version of React in the scripts that
				// were loaded above, let's ensure that's the case. 
				EnsureReactLoaded(engine);
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
					var contents = _fileSystem.ReadAsString(file);
					engine.Execute(contents);
				}
				catch (JsRuntimeException ex)
				{
					// We can't simply rethrow the exception here, as it's possible this is running
					// on a background thread (ie. as a response to a file changing). If we did
					// throw the exception here, it would terminate the entire process. Instead,
					// save the exception, and then just rethrow it later when getting the engine.
					_scriptLoadException = new ReactScriptLoadException(string.Format(
						"Error while loading \"{0}\": {1}\r\nLine: {2}\r\nColumn: {3}",
						file,
						ex.Message,
						ex.LineNumber,
						ex.ColumnNumber
					));
				}
			}
		}

		/// <summary>
		/// Ensures that React has been correctly loaded into the specified engine.
		/// </summary>
		/// <param name="engine">Engine to check</param>
		private static void EnsureReactLoaded(IJsEngine engine)
		{
			var result = engine.CallFunction<bool>("ReactNET_initReact");
			if (!result)
			{
				throw new ReactNotInitialisedException(
					"React has not been loaded correctly. Please expose your version of React as global " +
					"variables named 'React', 'ReactDOM' and 'ReactDOMServer', or enable the " +
					"'LoadReact' configuration option to use the built-in version of React. See " +
					"http://reactjs.net/guides/byo-react.html for more information."
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
		public virtual IJsEngine GetEngine()
		{
			EnsureValidState();
			return _pool.GetEngine();
		}

		/// <summary>
		/// Returns an engine to the pool so it can be reused
		/// </summary>
		/// <param name="engine">Engine to return</param>
		public virtual void ReturnEngineToPool(IJsEngine engine)
		{
			// This could be called from ReactEnvironment.Dispose if that class is disposed after 
			// this class. Let's just ignore this if it's disposed.
			if (!_disposed)
			{
				_pool.ReturnEngineToPool(engine);	
			}
		}

		/// <summary>
		/// Gets a factory for the most appropriate JavaScript engine for the current environment.
		/// The first functioning JavaScript engine with the lowest priority will be used.
		/// </summary>
		/// <returns>Function to create JavaScript engine</returns>
		private static Func<IJsEngine> GetFactory(IEnumerable<Registration> availableFactories, bool allowMsie)
		{
			var availableEngineFactories = availableFactories
				.OrderBy(x => x.Priority)
				.Select(x => x.Factory);
			foreach (var engineFactory in availableEngineFactories)
			{
				IJsEngine engine = null;
				try
				{
					engine = engineFactory();
					if (EngineIsUsable(engine, allowMsie))
					{
						// Success! Use this one.
						return engineFactory;
					}
				}
				catch (Exception ex)
				{
					// This engine threw an exception, try the next one
					Trace.WriteLine(string.Format("Error initialising {0}: {1}", engineFactory, ex));
				}
				finally
				{
					if (engine != null)
					{
						engine.Dispose();
					}
				}
			}

			// Epic fail, none of the engines worked. Nothing we can do now.
			// Throw an error relevant to the engine they should be able to use.
			if (JavaScriptEngineUtils.EnvironmentSupportsClearScript())
			{
				JavaScriptEngineUtils.EnsureEngineFunctional<V8JsEngine, ClearScriptV8InitialisationException>(
					ex => new ClearScriptV8InitialisationException(ex)
				);
			}
			else if (JavaScriptEngineUtils.EnvironmentSupportsVroomJs())
			{
				JavaScriptEngineUtils.EnsureEngineFunctional<VroomJsEngine, VroomJsInitialisationException>(
					ex => new VroomJsInitialisationException(ex.Message)
				);
			}
			throw new ReactEngineNotFoundException();
		}

		/// <summary>
		/// Performs a sanity check to ensure the specified engine type is usable.
		/// </summary>
		/// <param name="engine">Engine to test</param>
		/// <param name="allowMsie">Whether the MSIE engine can be used</param>
		/// <returns></returns>
		private static bool EngineIsUsable(IJsEngine engine, bool allowMsie)
		{
			// Perform a sanity test to ensure this engine is usable
			var isUsable = engine.Evaluate<int>("1 + 1") == 2;
			var isMsie = engine is MsieJsEngine;
			return isUsable && (!isMsie || allowMsie);
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

		/// <summary>
		/// Represents a factory for a supported JavaScript engine. 
		/// </summary>
		public class Registration
		{
			/// <summary>
			/// Gets or sets the factory for this JavaScript engine
			/// </summary>
			public Func<IJsEngine> Factory { get; set; }

			/// <summary>
			/// Gets or sets the priority for this JavaScript engine. Engines with lower priority
			/// are preferred.
			/// </summary>
			public int Priority { get; set; }
		}
	}
}
