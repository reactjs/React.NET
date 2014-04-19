/*
 *  Copyright (c) 2014, Facebook, Inc.
 *  All rights reserved.
 *
 *  This source code is licensed under the BSD-style license found in the
 *  LICENSE file in the root directory of this source tree. An additional grant 
 *  of patent rights can be found in the PATENTS file in the same directory.
 */

using System;
using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using React.Exceptions;

namespace React.Tests.Core
{
	[TestFixture]
	public class JsxTransformerTests
	{
		private Mock<IReactEnvironment> _environment;
		private Mock<ICache> _cache;
		private Mock<IFileSystem> _fileSystem;
		private Mock<IFileCacheHash> _fileCacheHash; 
		private JsxTransformer _jsxTransformer;

		[SetUp]
		public void SetUp()
		{
			_environment = new Mock<IReactEnvironment>();
			_environment.Setup(x => x.EngineSupportsJsxTransformer).Returns(true);

			_cache = new Mock<ICache>();
			_fileSystem = new Mock<IFileSystem>();
			_fileSystem.Setup(x => x.MapPath(It.IsAny<string>())).Returns<string>(x => x);

			_fileCacheHash = new Mock<IFileCacheHash>();

			_jsxTransformer = new JsxTransformer(
				_environment.Object,
				_cache.Object,
				_fileSystem.Object,
				_fileCacheHash.Object
			);
		}

		[Test]
		public void ShouldNotTransformJsxIfNoAnnotationPresent()
		{
			const string input = "<div>Hello World</div>";

			var output = _jsxTransformer.TransformJsx(input);
			Assert.AreEqual(input, output);
		}

		[Test]
		public void ShouldTransformJsxIfAnnotationPresent()
		{
			const string input = "/** @jsx React.DOM */ <div>Hello World</div>";
			_jsxTransformer.TransformJsx(input);

			_environment.Verify(x => x.ExecuteWithLargerStackIfRequired<string>(
				"ReactNET_transform",
				"/** @jsx React.DOM */ <div>Hello World</div>"
			));
		}

		[Test]
		public void ShouldWrapExceptionsInJsxExeption()
		{
			_environment.Setup(x => x.ExecuteWithLargerStackIfRequired<string>(
				"ReactNET_transform",
				"/** @jsx React.DOM */ <div>Hello World</div>"
			)).Throws(new Exception("Something broke..."));

			const string input = "/** @jsx React.DOM */ <div>Hello World</div>";
			Assert.Throws<JsxException>(() => _jsxTransformer.TransformJsx(input));
		}

		[Test]
		public void ShouldThrowIfEngineNotSupported()
		{
			_environment.Setup(x => x.EngineSupportsJsxTransformer).Returns(false);

			Assert.Throws<JsxUnsupportedEngineException>(() =>
			{
				_jsxTransformer.TransformJsx("/** @jsx React.DOM */ <div>Hello world</div>");
			});
		}

		[Test]
		public void ShouldUseCacheProvider()
		{
			_cache.Setup(x => x.GetOrInsert(
				/*key*/ "JSX_foo.jsx",
				/*slidingExpiration*/ It.IsAny<TimeSpan>(),
				/*getData*/ It.IsAny<Func<string>>(),
				/*cacheDependencyKeys*/ It.IsAny<IEnumerable<string>>(),
				/*cacheDependencyFiles*/ It.IsAny<IEnumerable<string>>()				
			)).Returns("/* cached */");

			var result = _jsxTransformer.TransformJsxFile("foo.jsx");
			Assert.AreEqual("/* cached */", result);
		}

		[Test]
		public void ShouldUseFileSystemCacheIfHashValid()
		{
			SetUpEmptyCache();
			_fileSystem.Setup(x => x.FileExists("foo.generated.js")).Returns(true);
			_fileSystem.Setup(x => x.ReadAsString("foo.generated.js")).Returns("/* filesystem cached */");
			_fileCacheHash.Setup(x => x.ValidateHash(It.IsAny<string>(), It.IsAny<string>())).Returns(true);
			
			var result = _jsxTransformer.TransformJsxFile("foo.jsx");
			Assert.AreEqual("/* filesystem cached */", result);
		}

		[Test]
		public void ShouldTransformJsxIfFileCacheHashInvalid()
		{
			SetUpEmptyCache();
			_fileSystem.Setup(x => x.FileExists("foo.generated.js")).Returns(true);
			_fileSystem.Setup(x => x.ReadAsString("foo.generated.js")).Returns("/* filesystem cached invalid */");
			_fileSystem.Setup(x => x.ReadAsString("foo.jsx")).Returns("/** @jsx React.DOM */ <div>Hello World</div>");
			_fileCacheHash.Setup(x => x.ValidateHash(It.IsAny<string>(), It.IsAny<string>())).Returns(false);

			_jsxTransformer.TransformJsxFile("foo.jsx");
			_environment.Verify(x => x.ExecuteWithLargerStackIfRequired<string>(
				"ReactNET_transform",
				"/** @jsx React.DOM */ <div>Hello World</div>"
			));
		}

		[Test]
		public void ShouldTransformJsxIfNoCache()
		{
			SetUpEmptyCache();
			_fileSystem.Setup(x => x.FileExists("foo.generated.js")).Returns(false);
			_fileSystem.Setup(x => x.ReadAsString("foo.jsx")).Returns("/** @jsx React.DOM */ <div>Hello World</div>");

			_jsxTransformer.TransformJsxFile("foo.jsx");
			_environment.Verify(x => x.ExecuteWithLargerStackIfRequired<string>(
				"ReactNET_transform",
				"/** @jsx React.DOM */ <div>Hello World</div>"
			));
		}

		[Test]
		public void ShouldSaveTransformationResult()
		{
			_fileSystem.Setup(x => x.ReadAsString("foo.jsx")).Returns("/** @jsx React.DOM */ <div>Hello World</div>");
			_environment.Setup(x => x.ExecuteWithLargerStackIfRequired<string>(
				"ReactNET_transform",
				"/** @jsx React.DOM */ <div>Hello World</div>"
			)).Returns("React.DOM.div('Hello World')");

			string result = null;
			_fileSystem.Setup(x => x.WriteAsString("foo.generated.js", It.IsAny<string>())).Callback(
				(string filename, string contents) => result = contents
			);

			var resultFilename = _jsxTransformer.TransformAndSaveJsxFile("foo.jsx");
			Assert.AreEqual("foo.generated.js", resultFilename);
			StringAssert.EndsWith("React.DOM.div('Hello World')", result);
		}

		private void SetUpEmptyCache()
		{
			_cache.Setup(x => x.GetOrInsert(
				/*key*/ "JSX_foo.jsx",
				/*slidingExpiration*/ It.IsAny<TimeSpan>(),
				/*getData*/ It.IsAny<Func<string>>(),
				/*cacheDependencyKeys*/ It.IsAny<IEnumerable<string>>(),
				/*cacheDependencyFiles*/ It.IsAny<IEnumerable<string>>()
			))
			.Returns((
				string key, 
				TimeSpan slidingExpiration, 
				Func<string> getData, 
				IEnumerable<string> cacheDependencyFiles, 
				IEnumerable<string> cacheDependencyKeys
			) => getData());
		}
	}
}
