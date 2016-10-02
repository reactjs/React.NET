/*
 *  Copyright (c) 2014-Present, Facebook, Inc.
 *  All rights reserved.
 *
 *  This source code is licensed under the BSD-style license found in the
 *  LICENSE file in the root directory of this source tree. An additional grant 
 *  of patent rights can be found in the PATENTS file in the same directory.
 */

using NUnit.Framework;

namespace React.Tests.Core
{
	[TestFixture]
	public class FileSystemBaseTests
	{
		[TestCase("~/Test.txt", "C:\\Test.txt")]
		[TestCase("~/Scripts/lol.js", "C:\\Scripts\\lol.js")]
		public void ToRelativePath(string expected, string input)
		{
			var fileSystem = new TestFileSystem();
			Assert.AreEqual(expected, fileSystem.ToRelativePath(input));
		}

		private class TestFileSystem : FileSystemBase
		{
			public override string MapPath(string relativePath)
			{
				return "C:\\" + relativePath.Replace("~/", string.Empty);
			}
		}
	}
}
