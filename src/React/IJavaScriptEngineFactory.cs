using System;
using JavaScriptEngineSwitcher.Core;

namespace React
{
	/// <summary>
	/// Handles creation of JavaScript engines. All methods are thread-safe.
	/// </summary>
	public interface IJavaScriptEngineFactory
	{
		/// <summary>
		/// Gets the JavaScript engine for the current thread
		/// </summary>
		/// <param name="onNewEngine">
		/// Called if a brand new JavaScript engine is being created for this thread.
		/// Should handle initialisation.
		/// </param>
		/// <returns>The JavaScript engine</returns>
		IJsEngine GetEngineForCurrentThread(Action<IJsEngine> onNewEngine);

		/// <summary>
		/// Disposes the JavaScript engine for the current thread.
		/// </summary>
		void DisposeEngineForCurrentThread();
	}
}