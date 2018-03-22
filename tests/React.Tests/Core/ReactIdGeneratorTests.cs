/*
 *  Copyright (c) 2016-Present, Facebook, Inc.
 *  All rights reserved.
 *
 *  This source code is licensed under the BSD-style license found in the
 *  LICENSE file in the root directory of this source tree. An additional grant 
 *  of patent rights can be found in the PATENTS file in the same directory.
 */

using System;
using Xunit;

namespace React.Tests.Core
{
	public class ReactIdGeneratorTests
	{
		[Fact]
		public void GuidShouldHaveFixedRange()
		{
			var shortGuid = ReactIdGenerator.Generate();

			Assert.Equal(19, shortGuid.Length);
		}

		[Fact]
		public void ShouldStartsWithReact()
		{
			var shortGuid = ReactIdGenerator.Generate();

			Assert.StartsWith("react_", shortGuid);
		}

		[Fact]
		public void TwoGuidsShouldNotEqual()
		{
			var shortGuid1 = ReactIdGenerator.Generate();
			var shortGuid2 = ReactIdGenerator.Generate();

			Assert.NotEqual(shortGuid1, shortGuid2);
		}
	}
}
