/*
 * Copyright (c) Facebook, Inc. and its affiliates.
 *
 * This source code is licensed under the MIT license found in the
 * LICENSE file in the root directory of this source tree.
 */

using Xunit;

namespace React.Tests.Core
{
	public class FileSystemExtensionsTests
	{
        [Theory]
        [InlineData("*.txt", true)]
		[InlineData("foo?.js", true)]
		[InlineData("first\\second\\third\\*.js", true)]
		[InlineData("lol.js", false)]
		[InlineData("", false)]
		[InlineData("hello\\world.js", false)]
		public void IsGlobPattern(string input, bool expected)
		{
			Assert.Equal(expected, input.IsGlobPattern());
		}
	}
}
