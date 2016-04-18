/*
 *  Copyright (c) 2014-Present, Facebook, Inc.
 *  All rights reserved.
 *
 *  This source code is licensed under the BSD-style license found in the
 *  LICENSE file in the root directory of this source tree. An additional grant 
 *  of patent rights can be found in the PATENTS file in the same directory.
 */

using System;
using System.Diagnostics;

namespace Cassette.React
{
	/// <summary>
	/// Utility methods for interacting with Cassette in a MSBuild environment.
	/// </summary>
	public static class MSBuildUtils
	{
		/// <summary>
		/// Determines if the current process is MSBuild
		/// </summary>
		/// <returns><c>true</c> if we are currently in MSBuild</returns>
		public static bool IsInMSBuild()
		{
			try
			{
				return Process.GetCurrentProcess().ProcessName.StartsWith("MSBuild");
			}
			catch (Exception)
			{
				return false;
			}
		}
	}
}
