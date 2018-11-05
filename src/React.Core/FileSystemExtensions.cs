/*
 * Copyright (c) Facebook, Inc. and its affiliates.
 *
 * This source code is licensed under the MIT license found in the
 * LICENSE file in the root directory of this source tree.
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
