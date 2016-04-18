/*
 *  Copyright (c) 2016-present, Facebook, Inc.
 *  All rights reserved.
 *
 *  This source code is licensed under the BSD-style license found in the
 *  LICENSE file in the root directory of this source tree. An additional grant 
 *  of patent rights can be found in the PATENTS file in the same directory.
 */

using NUnit.Framework;
using React.Owin;

namespace React.Tests.Owin
{
	[TestFixture]
	public class EntryAssemblyFileSystemTests
	{
		[TestCase("C:\\", "~/", "C:\\")]
		[TestCase("C:\\", "~/foo/bar.js", "C:\\foo\\bar.js")]
		public void MapPath(string rootPath, string relativePath, string expected)
		{
			var fileSystem = new EntryAssemblyFileSystem(rootPath);
			var result = fileSystem.MapPath(relativePath);
			Assert.AreEqual(expected, result);
		}
	}
}
