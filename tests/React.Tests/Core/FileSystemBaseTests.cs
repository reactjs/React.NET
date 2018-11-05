/*
 * Copyright (c) Facebook, Inc. and its affiliates.
 *
 * This source code is licensed under the MIT license found in the
 * LICENSE file in the root directory of this source tree.
 */

using Xunit;

namespace React.Tests.Core
{
	public class FileSystemBaseTests
	{
        [Theory]
		[InlineData("~/Test.txt", "C:\\Test.txt")]
		[InlineData("~/Scripts/lol.js", "C:\\Scripts\\lol.js")]
		public void ToRelativePath(string expected, string input)
		{
			var fileSystem = new TestFileSystem();
			Assert.Equal(expected, fileSystem.ToRelativePath(input));
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
