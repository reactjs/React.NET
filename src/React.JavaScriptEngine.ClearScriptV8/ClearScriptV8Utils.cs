/*
 *  Copyright (c) 2014-2015, Facebook, Inc.
 *  All rights reserved.
 *
 *  This source code is licensed under the BSD-style license found in the
 *  LICENSE file in the root directory of this source tree. An additional grant 
 *  of patent rights can be found in the PATENTS file in the same directory.
 */

using System;
using JavaScriptEngineSwitcher.V8;
using React.Exceptions;

namespace React.JavaScriptEngine.ClearScriptV8
{
	/// <summary>
	/// Utility methods for VroomJs JavaScript engine
	/// </summary>
	public static class ClearScriptV8Utils
	{
		/// <summary>
		/// Determines if the current environment supports the ClearScript V8 engine
		/// </summary>
		/// <returns></returns>
		public static bool IsEnvironmentSupported()
		{
			return Environment.OSVersion.Platform == PlatformID.Win32NT;
		}

		/// <summary>
		/// If the user is explicitly referencing this assembly, they probably want to use it.
		/// Attempt to use the engine and throw an exception if it doesn't work.
		/// </summary>
		public static void EnsureEngineFunctional()
		{
			int result = 0;
			try
			{
				using (var engine = new V8JsEngine())
				{
					result = engine.Evaluate<int>("1 + 1");
				}
			}
			catch (Exception ex)
			{
				throw new ClearScriptV8InitialisationException(ex.Message);
			}

			if (result != 2)
			{
				throw new ReactException("Mathematics is broken. 1 + 1 = " + result);
			}
		}
	}
}
