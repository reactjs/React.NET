using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using JavaScriptEngineSwitcher.Core;
using JavaScriptEngineSwitcher.Jint;
using JavaScriptEngineSwitcher.Msie;
using JavaScriptEngineSwitcher.Msie.Configuration;
using React.Exceptions;

namespace React
{
	/// <summary>
	/// Handles creation of JavaScript engines. All methods are thread-safe.
	/// </summary>
	public class JavaScriptEngineFactory : IDisposable, IJavaScriptEngineFactory
	{
		/// <summary>
		/// Function used to create new JavaScript engine instances.
		/// </summary>
		private readonly Func<IJsEngine> _factory; 
		/// <summary>
		/// Contains all current JavaScript engine instances. One per thread, keyed on thread ID.
		/// </summary>
		private readonly ConcurrentDictionary<int, IJsEngine> _engines 
			= new ConcurrentDictionary<int, IJsEngine>();

		/// <summary>
		/// Initializes a new instance of the <see cref="JavaScriptEngineFactory"/> class.
		/// </summary>
		public JavaScriptEngineFactory()
		{
			_factory = GetFactory();
		}

		/// <summary>
		/// Gets the JavaScript engine for the current thread
		/// </summary>
		/// <param name="onNewEngine">
		/// Called if a brand new JavaScript engine is being created for this thread.
		/// Should handle initialisation.
		/// </param>
		/// <returns>The JavaScript engine</returns>
		public IJsEngine GetEngineForCurrentThread(Action<IJsEngine> onNewEngine = null)
		{
			return _engines.GetOrAdd(Thread.CurrentThread.ManagedThreadId, id =>
			{
				var engine = _factory();
				if (onNewEngine != null)
				{
					onNewEngine(engine);
				}
				return engine;
			});
		}

		/// <summary>
		/// Disposes the JavaScript engine for the current thread.
		/// </summary>
		public void DisposeEngineForCurrentThread()
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
		/// Gets a factory for the most appropriate JavaScript engine for the current environment
		/// </summary>
		/// <returns>Function to create JavaScript engine</returns>
		private static Func<IJsEngine> GetFactory()
		{
			var availableEngineFactories = new List<Func<IJsEngine>>
			{
				() => new MsieJsEngine(new MsieConfiguration { EngineMode = JsEngineMode.ChakraActiveScript }),
				() => new MsieJsEngine(new MsieConfiguration { EngineMode = JsEngineMode.Classic }),
				() => new JintJsEngine()
			};
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
		public void Dispose()
		{
			foreach (var engine in _engines)
			{
				if (engine.Value != null)
				{
					engine.Value.Dispose();
				}
			}
		}
	}
}
