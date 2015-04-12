/*
 *  Copyright (c) 2014-2015, Facebook, Inc.
 *  All rights reserved.
 *
 *  This source code is licensed under the BSD-style license found in the
 *  LICENSE file in the root directory of this source tree. An additional grant 
 *  of patent rights can be found in the PATENTS file in the same directory.
 */

using System.IO;

namespace React
{
	/// <summary>
	/// Extension methods relating to file system paths.
	/// </summary>
	public static class FileSystemExtensions
	{
		/// <summary>
		/// Determines if the specified string is a glob pattern that can be used with 
		/// <see cref="Directory.GetFiles(string, string)"/>.
		/// </summary>
		/// <param name="input">String</param>
		/// <returns><c>true</c> if the specified string is a glob pattern</returns>
		public static bool IsGlobPattern(this string input)
		{
			return input.Contains("*") || input.Contains("?");
		}
	}
}
