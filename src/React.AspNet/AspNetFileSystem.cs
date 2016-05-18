/*
 *  Copyright (c) 2015, Facebook, Inc.
 *  All rights reserved.
 *
 *  This source code is licensed under the BSD-style license found in the
 *  LICENSE file in the root directory of this source tree. An additional grant 
 *  of patent rights can be found in the PATENTS file in the same directory.
 */

using System.IO;
using Microsoft.AspNetCore.Hosting;

namespace React.AspNet
{
	/// <summary>
	/// Handles file system functionality, such as reading files. Maps all paths from 
	/// application-relative (~/...) to full paths using ASP.NET's MapPath method.
	/// </summary>
	public class AspNetFileSystem : FileSystemBase
	{
		private readonly IHostingEnvironment _hostingEnv;

		/// <summary>
		/// Initializes a new instance of the <see cref="AspNetFileSystem"/> class.
		/// </summary>
		/// <param name="hostingEnv">The ASP.NET 5 hosting environment</param>
		public AspNetFileSystem(IHostingEnvironment hostingEnv)
		{
			_hostingEnv = hostingEnv;
		}

		/// <summary>
		/// Converts a path from an application relative path (~/...) to a full filesystem path
		/// </summary>
		/// <param name="relativePath">App-relative path of the file</param>
		/// <returns>Full path of the file</returns>
		public override string MapPath(string relativePath)
		{
			relativePath = relativePath.TrimStart('~').TrimStart('/');
			return Path.Combine(_hostingEnv.WebRootPath, relativePath);
		}
	}
}
