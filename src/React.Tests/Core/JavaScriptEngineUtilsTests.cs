/*
 *  Copyright (c) 2015, Facebook, Inc.
 *  All rights reserved.
 *
 *  This source code is licensed under the BSD-style license found in the
 *  LICENSE file in the root directory of this source tree. An additional grant 
 *  of patent rights can be found in the PATENTS file in the same directory.
 */

using JavaScriptEngineSwitcher.Core;
using Moq;
using NUnit.Framework;
using React.Exceptions;

namespace React.Tests.Core
{
	[TestFixture]
	public class JavaScriptEngineUtilsTests
	{
		[Test]
		public void CallFunctionReturningJsonHandlesScalarReturnValue()
		{
			var engine = new Mock<IJsEngine>();
			engine.Object.CallFunctionReturningJson<int>("hello");
			engine.Verify(x => x.CallFunction<int>("hello"));
		}

		[Test]
		public void CallFunctionReturningJsonHandlesJson()
		{
			var engine = new Mock<IJsEngine>();
			engine.Setup(x => x.CallFunction<string>("hello")).Returns("{\"message\":\"Hello World\"}");
			var result = engine.Object.CallFunctionReturningJson<Example>("hello");
			Assert.AreEqual("Hello World", result.Message);
		}

		[Test]
		public void CallFunctionReturningJsonThrowsOnInvalidJson()
		{
			var engine = new Mock<IJsEngine>();
			engine.Setup(x => x.CallFunction<string>("hello")).Returns("lol wut this is not json '\"");
			Assert.Throws<ReactException>(() => engine.Object.CallFunctionReturningJson<Example>("hello"));
		}

        private class Example
		{
			public string Message { get; set; }
		}
	}
}
