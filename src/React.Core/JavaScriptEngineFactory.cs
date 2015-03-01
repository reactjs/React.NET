using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
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
		/// Used to recycle the JavaScript engine pool when relevant JavaScript files are modified.
		/// </summary>
		protected readonly FileSystemWatcher _watcher;
		/// <summary>
		/// Names of all the files that are loaded into the JavaScript engine. If any of these 
		/// files are changed, the engines should be recycled
		/// </summary>
		protected readonly ISet<string> _watchedFiles;
		/// <summary>
		/// Timer for debouncing pool recycling
		/// </summary>
		protected readonly Timer _resetPoolTimer;
		/// <summary>
		/// Whether this class has been disposed.
		/// </summary>
		protected bool _disposed;

		/// <summary>
		/// Time period to debounce file system changed events, in milliseconds.
		/// </summary>
		protected const int DEBOUNCE_TIMEOUT = 25;

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
			_factory = GetFactory(availableFactories);
			if (_config.ReuseJavaScriptEngines)
			{
				_pool = CreatePool();
				_resetPoolTimer = new Timer(OnResetPoolTimer);
				_watchedFiles = new HashSet<string>(_config.Scripts.Select(
					fileName => _fileSystem.MapPath(fileName).ToLowerInvariant()
				));
				try
				{
					// Attempt to initialise a FileSystemWatcher so we can recycle the JavaScript
					// engine pool when files are changed.
					_watcher = new FileSystemWatcher
					{
						Path = _fileSystem.MapPath("~"),
						IncludeSubdirectories = true,
						EnableRaisingEvents = true,
					};
					_watcher.Changed += OnFileChanged;
					_watcher.Created += OnFileChanged;
					_watcher.Deleted += OnFileChanged;
					_watcher.Renamed += OnFileChanged;
				}
				catch (Exception ex)
				{
					// Can't use FileSystemWatcher (eg. not running in Full Trust)
					Trace.WriteLine("Unable to initialise FileSystemWatcher: " + ex.Message);
				}
			}
		}

		/// <summary>
		/// Creates a new JavaScript engine pool.
		/// </summary>
		protected virtual IJsPool CreatePool()
		{
			var poolConfig = new JsPoolConfig
			{
				EngineFactory = _factory,
				Initializer = InitialiseEngine,
			};
			if (_config.MaxEngines != null)
			{
				poolConfig.MaxEngines = _config.MaxEngines.Value;
			}
			if (_config.StartEngines != null)
			{
				poolConfig.StartEngines = _config.StartEngines.Value;
			}

			return new JsPool(poolConfig);
		}

		/// <summary>
		/// Loads standard React and JSXTransformer scripts into the engine.
		/// </summary>
		protected virtual void InitialiseEngine(IJsEngine engine)
		{
			var thisAssembly = typeof(ReactEnvironment).Assembly;
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
		/// Gets the JavaScript engine for the current thread. It is recommended to use 
		/// <see cref="GetEngine"/> instead, which will pool/reuse engines.
		/// </summary>
		/// <returns>The JavaScript engine</returns>
		public virtual IJsEngine GetEngineForCurrentThread()
		{
			EnsureNotDisposed();
			return _engines.GetOrAdd(Thread.CurrentThread.ManagedThreadId, id =>
			{
				var engine = _factory();
				InitialiseEngine(engine);
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
			EnsureNotDisposed();
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
		private static Func<IJsEngine> GetFactory(IEnumerable<Registration> availableFactories)
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
					// Perform a sanity test to ensure this engine is usable
					if (engine.Evaluate<int>("1 + 1") == 2)
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
			throw new ReactException("No usable JavaScript engine found :(");
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
		/// Handles events fired when any files are changed.
		/// </summary>
		/// <param name="sender">The sender</param>
		/// <param name="args">The <see cref="FileSystemEventArgs"/> instance containing the event data</param>
		protected virtual void OnFileChanged(object sender, FileSystemEventArgs args)
		{
			if (_watchedFiles.Contains(args.FullPath.ToLowerInvariant()))
			{
				// Use a timer so multiple changes only result in a single reset.
				_resetPoolTimer.Change(DEBOUNCE_TIMEOUT, Timeout.Infinite);
			}
		}

		/// <summary>
		/// Called when any of the watched files have changed. Recycles the JavaScript engine pool
		/// so the files are all reloaded.
		/// </summary>
		/// <param name="state">Unused</param>
		protected virtual void OnResetPoolTimer(object state)
		{
			// Create the new pool before disposing the old pool so that _pool is never null.
			var oldPool = _pool;
			_pool = CreatePool();
			if (oldPool != null)
			{
				oldPool.Dispose();
			}
		}

		/// <summary>
		/// Ensures that this object has not been disposed.
		/// </summary>
		public void EnsureNotDisposed()
		{
			if (_disposed)
			{
				throw new ObjectDisposedException(GetType().Name);
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
