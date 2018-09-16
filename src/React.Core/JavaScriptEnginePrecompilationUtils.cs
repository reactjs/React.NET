using System;
using System.Diagnostics;
using System.Reflection;
using JavaScriptEngineSwitcher.Core;

namespace React
{
	/// <summary>
	/// Helper methods for pre-compilation features of the JavaScript engine environment.
	/// </summary>
	public static class JavaScriptEnginePrecompilationUtils
	{
		/// <summary>
		/// Cache key for the script resource pre-compilation
		/// </summary>
		private const string PRECOMPILED_JS_RESOURCE_CACHE_KEY = "PRECOMPILED_JS_RESOURCE_{0}";
		/// <summary>
		/// Cache key for the script file pre-compilation
		/// </summary>
		private const string PRECOMPILED_JS_FILE_CACHE_KEY = "PRECOMPILED_JS_FILE_{0}";
		/// <summary>
		/// Value that indicates whether a cache entry, that contains a precompiled script, should be
		/// evicted if it has not been accessed in a given span of time
		/// </summary>
		private readonly static TimeSpan PRECOMPILED_JS_CACHE_ENTRY_SLIDING_EXPIRATION = TimeSpan.FromMinutes(30);

		/// <summary>
		/// Tries to execute a code with pre-compilation.
		/// </summary>
		/// <param name="engine">Engine to execute code with pre-compilation</param>
		/// <param name="cache">Cache used for storing the pre-compiled scripts</param>
		/// <param name="fileSystem">File system wrapper</param>
		/// <param name="code">JavaScript code</param>
		/// <param name="path">Path to the file on which the executable code depends (required
		/// for cache management).</param>
		/// <returns>true if can perform a script pre-compilation; otherwise, false.</returns>
		public static bool TryExecuteWithPrecompilation(this IJsEngine engine, ICache cache, IFileSystem fileSystem,
			string code, string path)
		{
			if (!CheckPrecompilationAvailability(engine, cache))
			{
				return false;
			}

			var cacheKey = string.Format(PRECOMPILED_JS_FILE_CACHE_KEY, path);
			var precompiledScript = cache.Get<IPrecompiledScript>(cacheKey);

			if (precompiledScript == null)
			{
				precompiledScript = engine.Precompile(code, path);
				var fullPath = fileSystem.MapPath(path);
				cache.Set(
					cacheKey,
					precompiledScript,
					slidingExpiration: PRECOMPILED_JS_CACHE_ENTRY_SLIDING_EXPIRATION,
					cacheDependencyFiles: new[] { fullPath }
				);
			}

			engine.Execute(precompiledScript);

			return true;
		}

		/// <summary>
		/// Tries to execute a code from JavaScript file with pre-compilation.
		/// </summary>
		/// <param name="engine">Engine to execute code from JavaScript file with pre-compilation</param>
		/// <param name="cache">Cache used for storing the pre-compiled scripts</param>
		/// <param name="fileSystem">File system wrapper</param>
		/// <param name="path">Path to the JavaScript file</param>
		/// <returns>true if can perform a script pre-compilation; otherwise, false.</returns>
		public static bool TryExecuteFileWithPrecompilation(this IJsEngine engine, ICache cache,
			IFileSystem fileSystem, string path)
		{
			if (!CheckPrecompilationAvailability(engine, cache))
			{
				return false;
			}

			var cacheKey = string.Format(PRECOMPILED_JS_FILE_CACHE_KEY, path);
			var precompiledScript = cache.Get<IPrecompiledScript>(cacheKey);

			if (precompiledScript == null)
			{
				var contents = fileSystem.ReadAsString(path);
				precompiledScript = engine.Precompile(contents, path);
				var fullPath = fileSystem.MapPath(path);
				cache.Set(
					cacheKey,
					precompiledScript,
					slidingExpiration: PRECOMPILED_JS_CACHE_ENTRY_SLIDING_EXPIRATION,
					cacheDependencyFiles: new[] { fullPath }
				);
			}

			engine.Execute(precompiledScript);

			return true;
		}

		/// <summary>
		/// Tries to execute a code from embedded JavaScript resource with pre-compilation.
		/// </summary>
		/// <param name="engine">Engine to execute a code from embedded JavaScript resource with pre-compilation</param>
		/// <param name="cache">Cache used for storing the pre-compiled scripts</param>
		/// <param name="resourceName">The case-sensitive resource name</param>
		/// <param name="assembly">The assembly, which contains the embedded resource</param>
		/// <returns>true if can perform a script pre-compilation; otherwise, false.</returns>
		public static bool TryExecuteResourceWithPrecompilation(this IJsEngine engine, ICache cache,
			string resourceName, Assembly assembly)
		{
			if (!CheckPrecompilationAvailability(engine, cache))
			{
				return false;
			}

			var cacheKey = string.Format(PRECOMPILED_JS_RESOURCE_CACHE_KEY, resourceName);
			var precompiledScript = cache.Get<IPrecompiledScript>(cacheKey);

			if (precompiledScript == null)
			{
				precompiledScript = engine.PrecompileResource(resourceName, assembly);
				cache.Set(
					cacheKey,
					precompiledScript,
					slidingExpiration: PRECOMPILED_JS_CACHE_ENTRY_SLIDING_EXPIRATION
				);
			}

			engine.Execute(precompiledScript);

			return true;
		}

		/// <summary>
		/// Checks a availability of the script pre-compilation
		/// </summary>
		/// <param name="engine">Instance of the JavaScript engine</param>
		/// <param name="cache">Cache used for storing the pre-compiled scripts</param>
		/// <returns>true if the script pre-compilation is available; otherwise, false.</returns>
		private static bool CheckPrecompilationAvailability(IJsEngine engine, ICache cache)
		{
			if (!engine.SupportsScriptPrecompilation)
			{
				Trace.WriteLine(string.Format("The {0} version {1} does not support the script pre-compilation.",
					engine.Name, engine.Version));
				return false;
			}

			if (cache is NullCache)
			{
				Trace.WriteLine("Usage of script pre-compilation without caching does not make sense.");
				return false;
			}

			return true;
		}
	}
}
