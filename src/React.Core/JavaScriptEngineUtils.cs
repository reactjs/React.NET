/*
 *  Copyright (c) 2015, Facebook, Inc.
 *  All rights reserved.
 *
 *  This source code is licensed under the BSD-style license found in the
 *  LICENSE file in the root directory of this source tree. An additional grant 
 *  of patent rights can be found in the PATENTS file in the same directory.
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
		/// Determines if the current environment supports the VroomJs engine.
		/// </summary>
		/// <returns><c>true</c> if VroomJs is supported</returns>
		public static bool EnvironmentSupportsVroomJs()
		{
			return Environment.OSVersion.Platform == PlatformID.Unix;
		}

		/// <summary>
		/// Determines if the current environment supports the ClearScript V8 engine
		/// </summary>
		/// <returns><c>true</c> if ClearScript is supported</returns>
		public static bool EnvironmentSupportsClearScript()
		{
			return Environment.OSVersion.Platform == PlatformID.Win32NT;
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
