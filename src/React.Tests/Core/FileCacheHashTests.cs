/*
 *  Copyright (c) 2014-2015, Facebook, Inc.
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
	public class FileCacheHashTests
	{
		private const string SAMPLE_HASH = "0A4D55A8D778E5022FAB701977C5D840BBC486D0";

		[Test]
		public void TestCalculateHash()
		{
			var hash = new FileCacheHash();
			Assert.AreEqual(SAMPLE_HASH, hash.CalculateHash("Hello World"));
		}

		[Test]
		public void ValidateHashShouldReturnFalseForEmptyString()
		{
			var hash = new FileCacheHash();
			Assert.IsFalse(hash.ValidateHash(string.Empty, SAMPLE_HASH));
		}

		[Test]
		public void ValidateHashShouldReturnFalseForNull()
		{
			var hash = new FileCacheHash();
			Assert.IsFalse(hash.ValidateHash(null, SAMPLE_HASH));
		}

		[Test]
		public void ValidateHashShouldReturnFalseWhenNoHashPrefix()
		{
			var hash = new FileCacheHash();
			Assert.IsFalse(hash.ValidateHash("Hello World", SAMPLE_HASH));
		}

		[Test]
		public void ValidateHashShouldReturnFalseWhenHashDoesNotMatch()
		{
			var hash = new FileCacheHash();
			Assert.IsFalse(hash.ValidateHash("// @hash NOTCORRECT\nHello World", SAMPLE_HASH));
		}

		[Test]
		public void ValidateHashShouldReturnTrueWhenHashMatches()
		{
			var hash = new FileCacheHash();
			Assert.IsTrue(hash.ValidateHash("// @hash v3-" + SAMPLE_HASH + "\nHello World", SAMPLE_HASH));
		}
	}
}
