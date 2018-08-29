/*
 *  Copyright (c) 2015, Facebook, Inc.
 *  All rights reserved.
 *
 *  This source code is licensed under the BSD-style license found in the
 *  LICENSE file in the root directory of this source tree. An additional grant 
 *  of patent rights can be found in the PATENTS file in the same directory.
 */

using System;
using System.Diagnostics;
using System.Reflection;
using JavaScriptEngineSwitcher.Core;
using JavaScriptEngineSwitcher.Core.Helpers;
using Newtonsoft.Json;
using React.Exceptions;

namespace React
{
	/// <summary>
	/// Various helper methods for the JavaScript engine environment.
	/// </summary>
	public static class JavaScriptEngineUtils
	{
		/// <summary>
		/// Cache key for the script resource precompilation
		/// </summary>
		private const string PRECOMPILED_JS_RESOURCE_CACHE_KEY = "PRECOMPILED_JS_RESOURCE_{0}";
		/// <summary>
		/// Cache key for the script file precompilation
		/// </summary>
		private const string PRECOMPILED_JS_FILE_CACHE_KEY = "PRECOMPILED_JS_FILE_{0}";
		/// <summary>
		/// Value that indicates whether a cache entry, that contains a precompiled script, should be
		/// evicted if it has not been accessed in a given span of time
		/// </summary>
		private readonly static TimeSpan PRECOMPILED_JS_CACHE_ENTRY_SLIDING_EXPIRATION = TimeSpan.FromMinutes(30);

		/// <summary>
		/// Determines if the current environment supports the ClearScript V8 engine
		/// </summary>
		/// <returns><c>true</c> if ClearScript is supported</returns>
		public static bool EnvironmentSupportsClearScript()
		{
#if NET40
			return Environment.OSVersion.Platform == PlatformID.Win32NT;
#else
			return false;
#endif
		}

		/// <summary>
		/// Attempts to use the specified engine and throws an exception if it doesn't work.
		/// </summary>
		public static void EnsureEngineFunctional<TEngine, TException>(
			Func<Exception, TException> exceptionFactory
		) 
			where TEngine : IJsEngine, new()
			where TException : Exception
		{
			int result;
			try
			{
				using (var engine = new TEngine())
				{
					result = engine.Evaluate<int>("1 + 1");
				}
			}
			catch (Exception ex)
			{
				throw exceptionFactory(ex);
			}

			if (result != 2)
			{
				throw new ReactException("Mathematics is broken. 1 + 1 = " + result);
			}
		}

		/// <summary>
		/// Executes a code from JavaScript file.
		/// </summary>
		/// <param name="engine">Engine to execute code from JavaScript file</param>
		/// <param name="fileSystem">File system wrapper</param>
		/// <param name="path">Path to the JavaScript file</param>
		public static void ExecuteFile(this IJsEngine engine, IFileSystem fileSystem, string path)
		{
			var contents = fileSystem.ReadAsString(path);
			engine.Execute(contents, path);
		}

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

		/// <summary>
		/// Calls a JavaScript function using the specified engine. If <typeparamref name="T"/> is
		/// not a scalar type, the function is assumed to return a string of JSON that can be 
		/// parsed as that type.
		/// </summary>
		/// <typeparam name="T">Type returned by function</typeparam>
		/// <param name="engine">Engine to execute function with</param>
		/// <param name="function">Name of the function to execute</param>
		/// <param name="args">Arguments to pass to function</param>
		/// <returns>Value returned by function</returns>
		public static T CallFunctionReturningJson<T>(this IJsEngine engine, string function, params object[] args)
		{
			if (ValidationHelpers.IsSupportedType(typeof(T)))
			{
				// Type is supported directly (ie. a scalar type like string/int/bool)
				// Just execute the function directly.
				return engine.CallFunction<T>(function, args);
			}
			// The type is not a scalar type. Assume the function will return its result as
			// JSON.
			var resultJson = engine.CallFunction<string>(function, args);
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
	}
}
