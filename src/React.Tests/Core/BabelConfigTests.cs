/*
 *  Copyright (c) 2015, Facebook, Inc.
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
	public class BabelConfigTests
	{
		[Test]
		public void SerializesAllLoose()
		{
			var config = new BabelConfig
			{
				AllLoose = true,
			};
			var result = config.Serialize();
			Assert.AreEqual(@"{""blacklist"":[""strict""],""externalHelpers"":false,""optional"":null,""stage"":2,""loose"":""all""}", result);
		}

		[Test]
		public void SerializesSomeLoose()
		{
			var config = new BabelConfig
			{
				Loose = new[] { "foo" },
			};
			var result = config.Serialize();
			Assert.AreEqual(@"{""blacklist"":[""strict""],""externalHelpers"":false,""optional"":null,""stage"":2,""loose"":[""foo""]}", result);
		}
	}
}
