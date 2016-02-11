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
	public class FileSystemExtensionsTests
	{
		[TestCase("*.txt", true)]
		[TestCase("foo?.js", true)]
		[TestCase("first\\second\\third\\*.js", true)]
		[TestCase("lol.js", false)]
		[TestCase("", false)]
		[TestCase("hello\\world.js", false)]
		public void IsGlobPattern(string input, bool expected)
		{
			Assert.AreEqual(expected, input.IsGlobPattern());
		}
	}
}
