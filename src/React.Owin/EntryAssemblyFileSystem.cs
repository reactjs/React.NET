/*
 *  Copyright (c) 2014-Present, Facebook, Inc.
 *  All rights reserved.
 *
 *  This source code is licensed under the BSD-style license found in the
 *  LICENSE file in the root directory of this source tree. An additional grant 
 *  of patent rights can be found in the PATENTS file in the same directory.
 */

using System.IO;
using System.Reflection;

namespace React.Owin
{
	/// <summary>
	/// Implements React file system that maps "~" into entry assembly location.
	/// </summary>
	internal class EntryAssemblyFileSystem : FileSystemBase
	{
		private readonly string _rootPath;

		/// <summary>
		/// Initializes a new instance of the <see cref="EntryAssemblyFileSystem"/> class, using the
		/// entry assembly's location as the root directory.
		/// </summary>
		public EntryAssemblyFileSystem()
			: this(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location))
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="EntryAssemblyFileSystem"/> class, using the 
		/// specified path as the root directory.
		/// </summary>
		/// <param name="rootPath">The root path.</param>
		public EntryAssemblyFileSystem(string rootPath)
		{
			_rootPath = rootPath;
		}

		/// <summary>
		/// Converts a path from an application relative path (~/...) to a full filesystem path
		/// </summary>
		/// <param name="relativePath">App-relative path of the file</param>
		/// <returns>Full path of the file</returns>
		public override string MapPath(string relativePath)
		{
			if (relativePath.StartsWith("~"))
			{
				relativePath = relativePath
					.Replace("~/", string.Empty)
					.Replace('/', '\\');
				return Path.Combine(_rootPath, relativePath);
			}

			return relativePath;
		}
	}
}
