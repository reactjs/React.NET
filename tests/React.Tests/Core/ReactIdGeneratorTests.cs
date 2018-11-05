/*
 * Copyright (c) Facebook, Inc. and its affiliates.
 *
 * This source code is licensed under the MIT license found in the
 * LICENSE file in the root directory of this source tree.
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
			var generator = new ReactIdGenerator();

			var shortGuid = generator.Generate();

			Assert.Equal(19, shortGuid.Length);
		}

		[Fact]
		public void ShouldStartsWithReact()
		{
			var generator = new ReactIdGenerator();

			var shortGuid = generator.Generate();

			Assert.StartsWith("react_", shortGuid);
		}

		[Fact]
		public void TwoGuidsShouldNotEqual()
		{
			var generator = new ReactIdGenerator();

			var shortGuid1 = generator.Generate();
			var shortGuid2 = generator.Generate();

			Assert.NotEqual(shortGuid1, shortGuid2);
		}
	}
}
