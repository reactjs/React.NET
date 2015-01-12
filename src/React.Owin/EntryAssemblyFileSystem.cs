/*
 *  Copyright (c) 2014-2015, Facebook, Inc.
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
		public override string MapPath(string relativePath)
		{
			if (relativePath.StartsWith("~"))
				return Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), relativePath.Replace("~", string.Empty));

			return relativePath;
		}
	}
}
