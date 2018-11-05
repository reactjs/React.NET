/*
 * Copyright (c) Facebook, Inc. and its affiliates.
 *
 * This source code is licensed under the MIT license found in the
 * LICENSE file in the root directory of this source tree.
 */

using JavaScriptEngineSwitcher.Core;
using Moq;
using Xunit;
using React.Exceptions;

namespace React.Tests.Core
{
	public class JavaScriptEngineUtilsTests
	{
		[Fact]
		public void CallFunctionReturningJsonHandlesScalarReturnValue()
		{
			var engine = new Mock<IJsEngine>();
			engine.Object.CallFunctionReturningJson<int>("hello");
			engine.Verify(x => x.CallFunction<int>("hello"));
		}

		[Fact]
		public void CallFunctionReturningJsonHandlesJson()
		{
			var engine = new Mock<IJsEngine>();
			engine.Setup(x => x.CallFunction<string>("hello")).Returns("{\"message\":\"Hello World\"}");
			var result = engine.Object.CallFunctionReturningJson<Example>("hello");
			Assert.Equal("Hello World", result.Message);
		}

		[Fact]
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
