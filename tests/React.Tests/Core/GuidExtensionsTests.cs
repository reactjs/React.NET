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
	public class GuidExtensionsTests
	{
		[Fact]
		public void ToShortGuid()
		{
			var guid = Guid.Parse("c027191d-3785-485d-9fd7-5e0b376bd547");
			Assert.Equal("HRknwIU3XUif114LN2vVRw", guid.ToShortGuid());
		}
	}
}
