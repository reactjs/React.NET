/*
 *  Copyright (c) 2014-Present, Facebook, Inc.
 *  All rights reserved.
 *
 *  This source code is licensed under the BSD-style license found in the
 *  LICENSE file in the root directory of this source tree. An additional grant 
 *  of patent rights can be found in the PATENTS file in the same directory.
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
