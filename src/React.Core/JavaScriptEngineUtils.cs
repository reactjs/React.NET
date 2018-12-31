/*
 * Copyright (c) Facebook, Inc. and its affiliates.
 *
 * This source code is licensed under the MIT license found in the
 * LICENSE file in the root directory of this source tree.
 */

using System;
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
				), ex);
			}
		}
	}
}
