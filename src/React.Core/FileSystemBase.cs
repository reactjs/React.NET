/*
 *  Copyright (c) 2014-Present, Facebook, Inc.
 *  All rights reserved.
 *
 *  This source code is licensed under the BSD-style license found in the
 *  LICENSE file in the root directory of this source tree. An additional grant 
 *  of patent rights can be found in the PATENTS file in the same directory.
 */

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace React
{
	/// <summary>
	/// Handles file system functionality, such as reading files.
	/// </summary>
	abstract public class FileSystemBase : IFileSystem
	{
		/// <summary>
		/// Prefix for relative paths
		/// </summary>
		public const string RELATIVE_PREFIX = "~/";

		/// <summary>
		/// Converts a path from an application relative path (~/...) to a full filesystem path
		/// </summary>
		/// <param name="relativePath">App-relative path of the file</param>
		/// <returns>Full path of the file</returns>
		public abstract string MapPath(string relativePath);

		/// <summary>
		/// Converts a path from a full filesystem path to anan application relative path (~/...)
		/// </summary>
		/// <param name="absolutePath">Full path of the file</param>
		/// <returns>App-relative path of the file</returns>
		public virtual string ToRelativePath(string absolutePath)
		{
			var root = MapPath(RELATIVE_PREFIX);
			return absolutePath.Replace(root, RELATIVE_PREFIX).Replace('\\', '/');
		}

		/// <summary>
		/// Reads the contents of a file as a string.
		/// </summary>
		/// <param name="relativePath">App-relative path of the file</param>
		/// <returns>Contents of the file</returns>
		public virtual string ReadAsString(string relativePath)
		{
			return File.ReadAllText(MapPath(relativePath), Encoding.UTF8);
		}

		/// <summary>
		/// Writes a string to a file
		/// </summary>
		/// <param name="relativePath">App-relative path of the file</param>
		/// <param name="contents">Contents of the file</param>
		public virtual void WriteAsString(string relativePath, string contents)
		{
			File.WriteAllText(MapPath(relativePath), contents, Encoding.UTF8);
		}

		/// <summary>
		/// Determines if the specified file exists
		/// </summary>
		/// <param name="relativePath">App-relative path of the file</param>
		/// <returns><c>true</c> if the file exists</returns>
		public virtual bool FileExists(string relativePath)
		{
			return File.Exists(MapPath(relativePath));
		}

		/// <summary>
		/// Gets all the file paths that match the specified pattern
		/// </summary>
		/// <param name="glob">Pattern to search for (eg. "~/Scripts/*.js")</param>
		/// <returns>File paths that match the pattern</returns>
		public virtual IEnumerable<string> Glob(string glob)
		{
			var path = MapPath(Path.GetDirectoryName(glob));
			var searchPattern = Path.GetFileName(glob);
			return Directory.EnumerateFiles(path, searchPattern).Select(ToRelativePath);
		}
	}
}
