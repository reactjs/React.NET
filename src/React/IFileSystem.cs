/*
 *  Copyright (c) 2014, Facebook, Inc.
 *  All rights reserved.
 *
 *  This source code is licensed under the BSD-style license found in the
 *  LICENSE file in the root directory of this source tree. An additional grant 
 *  of patent rights can be found in the PATENTS file in the same directory.
 */

namespace React
{
	/// <summary>
	/// Handles file system functionality, such as reading files.
	/// </summary>
	public interface IFileSystem
	{
		/// <summary>
		/// Converts a path from an application relative path (~/...) to a full filesystem path
		/// </summary>
		/// <param name="relativePath">App-relative path of the file</param>
		/// <returns>Full path of the file</returns>
		string MapPath(string relativePath);

		/// <summary>
		/// Reads the contents of a file as a string.
		/// </summary>
		/// <param name="relativePath">App-relative path of the file</param>
		/// <returns>Contents of the file</returns>
		string ReadAsString(string relativePath);

		/// <summary>
		/// Writes a string to a file
		/// </summary>
		/// <param name="relativePath">App-relative path of the file</param>
		/// <param name="contents">Contents of the file</param>
		void WriteAsString(string relativePath, string contents);

		/// <summary>
		/// Determines if the specified file exists
		/// </summary>
		/// <param name="relativePath">App-relative path of the file</param>
		/// <returns><c>true</c> if the file exists</returns>
		bool FileExists(string relativePath);
	}
}
