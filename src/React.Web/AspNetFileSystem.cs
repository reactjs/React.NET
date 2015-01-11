﻿/*
 *  Copyright (c) 2014-2015, Facebook, Inc.
 *  All rights reserved.
 *
 *  This source code is licensed under the BSD-style license found in the
 *  LICENSE file in the root directory of this source tree. An additional grant 
 *  of patent rights can be found in the PATENTS file in the same directory.
 */

using System.Web;

namespace React.Web
{
	/// <summary>
	/// Handles file system functionality, such as reading files. Maps all paths from 
	/// application-relative (~/...) to full paths using ASP.NET's MapPath method.
	/// </summary>
	public class AspNetFileSystem : FileSystemBase
	{
		/// <summary>
		/// The ASP.NET server utilities
		/// </summary>
		private readonly HttpServerUtilityBase _serverUtility;

		/// <summary>
		/// Initializes a new instance of the <see cref="AspNetFileSystem"/> class.
		/// </summary>
		/// <param name="serverUtility">The server utility.</param>
		public AspNetFileSystem(HttpServerUtilityBase serverUtility)
		{
			_serverUtility = serverUtility;
		}

		/// <summary>
		/// Converts a path from an application relative path (~/...) to a full filesystem path
		/// </summary>
		/// <param name="relativePath">App-relative path of the file</param>
		/// <returns>Full path of the file</returns>
		public override string MapPath(string relativePath)
		{
			return _serverUtility.MapPath(relativePath);
		}
	}
}
