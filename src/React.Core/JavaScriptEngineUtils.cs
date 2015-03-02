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
	}
}
