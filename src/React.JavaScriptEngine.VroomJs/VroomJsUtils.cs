/*
 *  Copyright (c) 2014-2015, Facebook, Inc.
 *  All rights reserved.
 *
 *  This source code is licensed under the BSD-style license found in the
 *  LICENSE file in the root directory of this source tree. An additional grant 
 *  of patent rights can be found in the PATENTS file in the same directory.
 */

using System;
using React.Exceptions;

namespace React.JavaScriptEngine.VroomJs
{
	/// <summary>
	/// Utility methods for VroomJs JavaScript engine
	/// </summary>
	public static class VroomJsUtils
	{
		/// <summary>
		/// Determines if the current environment supports the VroomJs engine
		/// </summary>
		/// <returns></returns>
		public static bool IsEnvironmentSupported()
		{
			return Environment.OSVersion.Platform == PlatformID.Unix;
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
				using (var engine = new VroomJsEngine())
				{
					result = engine.Evaluate<int>("1 + 1");
				}
			}
			catch (Exception ex)
			{
				throw new VroomJsInitialisationException(ex.Message);
			}

			if (result != 2)
			{
				throw new ReactException("Mathematics is broken. 1 + 1 = " + result);
			}
		}
	}
}
