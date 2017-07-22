using JavaScriptEngineSwitcher.Core;
using JSPool;

namespace React
{
	/// <summary>
	/// Handles creation of JavaScript engines. All methods are thread-safe.
	/// </summary>
	public interface IJavaScriptEngineFactory
	{
		/// <summary>
		/// Gets the JavaScript engine for the current thread. It is recommended to use 
		/// <see cref="GetEngine"/> instead, which will pool/reuse engines.
		/// </summary>
		/// <returns>The JavaScript engine</returns>
		IJsEngine GetEngineForCurrentThread();

		/// <summary>
		/// Disposes the JavaScript engine for the current thread. This should only be used
		/// if the engine was acquired through <see cref="GetEngineForCurrentThread"/>.
		/// </summary>
		void DisposeEngineForCurrentThread();

		/// <summary>
		/// Gets a JavaScript engine from the pool.
		/// </summary>
		/// <returns>The JavaScript engine</returns>
		PooledJsEngine GetEngine();
	}
}