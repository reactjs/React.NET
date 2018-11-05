/*
 * Copyright (c) Facebook, Inc. and its affiliates.
 *
 * This source code is licensed under the MIT license found in the
 * LICENSE file in the root directory of this source tree.
 */
#if NET452

using Xunit;
using React.Owin;

namespace React.Tests.Owin
{
	public class EntryAssemblyFileSystemTests
	{
		[Theory]
		[InlineData("C:\\", "~/", "C:\\")]
		[InlineData("C:\\", "~/foo/bar.js", "C:\\foo\\bar.js")]
		public void MapPath(string rootPath, string relativePath, string expected)
		{
			var fileSystem = new EntryAssemblyFileSystem(rootPath);
			var result = fileSystem.MapPath(relativePath);
			Assert.Equal(expected, result);
		}
	}
}
#endif
