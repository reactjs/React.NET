/*
 *  Copyright (c) 2014, Facebook, Inc.
 *  All rights reserved.
 *
 *  This source code is licensed under the BSD-style license found in the
 *  LICENSE file in the root directory of this source tree. An additional grant 
 *  of patent rights can be found in the PATENTS file in the same directory.
 */

using System.IO;
using System.Text;

namespace React
{
	/// <summary>
	/// Handles file system functionality, such as reading files.
	/// </summary>
	abstract public class FileSystemBase : IFileSystem
	{
		/// <summary>
		/// Converts a path from an application relative path (~/...) to a full filesystem path
		/// </summary>
		/// <param name="relativePath">App-relative path of the file</param>
		/// <returns>Full path of the file</returns>
		public abstract string MapPath(string relativePath);

		/// <summary>
		/// Reads the contents of a file as a string.
		/// </summary>
		/// <param name="relativePath">App-relative path of the file</param>
		/// <returns>Contents of the file</returns>
		public string ReadAsString(string relativePath)
		{
			return File.ReadAllText(MapPath(relativePath), Encoding.UTF8);
		}

		/// <summary>
		/// Writes a string to a file
		/// </summary>
		/// <param name="relativePath">App-relative path of the file</param>
		/// <param name="contents">Contents of the file</param>
		public void WriteAsString(string relativePath, string contents)
		{
			File.WriteAllText(MapPath(relativePath), contents, Encoding.UTF8);
		}
	}
}
