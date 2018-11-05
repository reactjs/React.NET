/*
 * Copyright (c) Facebook, Inc. and its affiliates.
 *
 * This source code is licensed under the MIT license found in the
 * LICENSE file in the root directory of this source tree.
 */

using Xunit;

namespace React.Tests.Core
{
	public class FileCacheHashTests
	{
		private const string SAMPLE_HASH = "0A4D55A8D778E5022FAB701977C5D840BBC486D0";

		[Fact]
		public void TestCalculateHash()
		{
			var hash = new FileCacheHash();
			Assert.Equal(SAMPLE_HASH, hash.CalculateHash("Hello World"));
		}

		[Fact]
		public void ValidateHashShouldReturnFalseForEmptyString()
		{
			var hash = new FileCacheHash();
			Assert.False(hash.ValidateHash(string.Empty, SAMPLE_HASH));
		}

		[Fact]
		public void ValidateHashShouldReturnFalseForNull()
		{
			var hash = new FileCacheHash();
			Assert.False(hash.ValidateHash(null, SAMPLE_HASH));
		}

		[Fact]
		public void ValidateHashShouldReturnFalseWhenNoHashPrefix()
		{
			var hash = new FileCacheHash();
			Assert.False(hash.ValidateHash("Hello World", SAMPLE_HASH));
		}

		[Fact]
		public void ValidateHashShouldReturnFalseWhenHashDoesNotMatch()
		{
			var hash = new FileCacheHash();
			Assert.False(hash.ValidateHash("// @hash NOTCORRECT\nHello World", SAMPLE_HASH));
		}

		[Fact]
		public void ValidateHashShouldReturnTrueWhenHashMatches()
		{
			var hash = new FileCacheHash();
			Assert.True(hash.ValidateHash("// @hash v3-" + SAMPLE_HASH + "\nHello World", SAMPLE_HASH));
		}
	}
}
