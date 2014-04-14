/*
 *  Copyright (c) 2014, Facebook, Inc.
 *  All rights reserved.
 *
 *  This source code is licensed under the BSD-style license found in the
 *  LICENSE file in the root directory of this source tree. An additional grant 
 *  of patent rights can be found in the PATENTS file in the same directory.
 */

using Moq;
using React.Exceptions;
using Xunit;

namespace React.Tests.Core
{
	public class JsxTransformerTests
	{
		[Fact]
		public void ShouldNotTransformJsxIfNoAnnotationPresent()
		{
			var environment = new Mock<IReactEnvironment>();
			var cache = new Mock<ICache>();
			var fileSystem = new Mock<IFileSystem>();
			var jsxTransformer = new JsxTransformer(
				environment.Object, 
				cache.Object, 
				fileSystem.Object
			);
			const string input = "<div>Hello World</div>";

			var output = jsxTransformer.TransformJsx(input);
			Assert.Equal(input, output);
		}

		[Fact]
		public void ShouldTransformJsxIfAnnotationPresent()
		{
			var environment = new Mock<IReactEnvironment>();
			var cache = new Mock<ICache>();
			var fileSystem = new Mock<IFileSystem>();
			var jsxTransformer = new JsxTransformer(
				environment.Object,
				cache.Object,
				fileSystem.Object
			);
			environment.Setup(x => x.EngineSupportsJsxTransformer).Returns(true);
			const string input = "/** @jsx React.DOM */ <div>Hello World</div>";
			jsxTransformer.TransformJsx(input);

			environment.Verify(x => x.ExecuteWithLargerStackIfRequired<string>(
				@"global.JSXTransformer.transform(""/** @jsx React.DOM */ <div>Hello World</div>"").code"
			));
		}

		public void ShouldThrowIfEngineNotSupported()
		{
			var environment = new Mock<IReactEnvironment>();
			var cache = new Mock<ICache>();
			var fileSystem = new Mock<IFileSystem>();
			var jsxTransformer = new JsxTransformer(
				environment.Object,
				cache.Object,
				fileSystem.Object
			);
			environment.Setup(x => x.EngineSupportsJsxTransformer).Returns(false);

			Assert.Throws<JsxUnsupportedEngineException>(() =>
			{
				jsxTransformer.TransformJsx("/** @jsx React.DOM */ <div>Hello world</div>");
			});
		}
	}
}
