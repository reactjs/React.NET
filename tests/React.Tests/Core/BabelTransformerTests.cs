/*
 *  Copyright (c) 2014-Present, Facebook, Inc.
 *  All rights reserved.
 *
 *  This source code is licensed under the BSD-style license found in the
 *  LICENSE file in the root directory of this source tree. An additional grant 
 *  of patent rights can be found in the PATENTS file in the same directory.
 */

using System;
using Moq;
using React.Exceptions;
using Xunit;

namespace React.Tests.Core
{
	public class BabelTransformerTests
	{
		private readonly Mock<IReactEnvironment> _environment;
		private readonly Mock<ICache> _cache;
		private readonly Mock<IFileSystem> _fileSystem;
		private readonly Mock<IFileCacheHash> _fileCacheHash;
		private readonly Babel _babel;

		public BabelTransformerTests()
		{
			_environment = new Mock<IReactEnvironment>();

			_cache = new Mock<ICache>();
			_fileSystem = new Mock<IFileSystem>();
			_fileSystem.Setup(x => x.MapPath(It.IsAny<string>())).Returns<string>(x => x);

			// Per default the output file should not exist, then individual tests
			// can choose otherwise.
			_fileSystem.Setup(x => x.FileExists(It.IsAny<string>())).Returns(false);

			_fileCacheHash = new Mock<IFileCacheHash>();

			_babel = new Babel(
				_environment.Object,
				_cache.Object,
				_fileSystem.Object,
				_fileCacheHash.Object,
				ReactSiteConfiguration.Configuration
			);
		}

		[Fact]
		public void ShouldTransformJsx()
		{
			const string input = "<div>Hello World</div>";
			_babel.Transform(input);

			_environment.Verify(x => x.ExecuteWithBabel<string>(
				"ReactNET_transform",
				"<div>Hello World</div>",
				It.IsAny<string>(),
				"unknown" // file name
			));
		}

		[Fact]
		public void ShouldWrapExceptionsInJsxExeption()
		{
			_environment.Setup(x => x.ExecuteWithBabel<string>(
				"ReactNET_transform",
				"<div>Hello World</div>",
				It.IsAny<string>(),
				"unknown" // file name
			)).Throws(new Exception("Something broke..."));

			const string input = "<div>Hello World</div>";
			Assert.Throws<BabelException>(() => _babel.Transform(input));
		}

		[Fact]
		public void ShouldUseCacheProvider()
		{
			_cache.Setup(x => x.Get<JavaScriptWithSourceMap>("JSX_v3_foo.jsx", null)).Returns(new JavaScriptWithSourceMap
			{
				Code = "/* cached */"
			});

			var result = _babel.TransformFile("foo.jsx");
			Assert.Equal("/* cached */", result);
		}

		[Fact]
		public void ShouldUseFileSystemCacheIfHashValid()
		{
			SetUpEmptyCache();
			_fileSystem.Setup(x => x.FileExists("foo.generated.js")).Returns(true);
			_fileSystem.Setup(x => x.ReadAsString("foo.generated.js")).Returns("/* filesystem cached */");
			_fileCacheHash.Setup(x => x.ValidateHash(It.IsAny<string>(), It.IsAny<string>())).Returns(true);

			var result = _babel.TransformFile("foo.jsx");
			Assert.Equal("/* filesystem cached */", result);
		}

		[Fact]
		public void ShouldTransformJsxIfFileCacheHashInvalid()
		{
			SetUpEmptyCache();
			_fileSystem.Setup(x => x.FileExists("foo.generated.js")).Returns(true);
			_fileSystem.Setup(x => x.ReadAsString("foo.generated.js")).Returns("/* filesystem cached invalid */");
			_fileSystem.Setup(x => x.ReadAsString("foo.jsx")).Returns("<div>Hello World</div>");
			_fileCacheHash.Setup(x => x.ValidateHash(It.IsAny<string>(), It.IsAny<string>())).Returns(false);
			_environment.Setup(x => x.ExecuteWithBabel<JavaScriptWithSourceMap>(
				"ReactNET_transform_sourcemap",
				It.IsAny<string>(),
				It.IsAny<string>(), // Babel config
				"foo.jsx" // File name
			)).Returns(new JavaScriptWithSourceMap { Code = "React.DOM.div('Hello World')" });

			var result = _babel.TransformFile("foo.jsx");
			Assert.EndsWith("React.DOM.div('Hello World')", result);
		}

		[Fact]
		public void ShouldTransformJsxIfNoCache()
		{
			SetUpEmptyCache();
			_fileSystem.Setup(x => x.FileExists("foo.generated.js")).Returns(false);
			_fileSystem.Setup(x => x.ReadAsString("foo.jsx")).Returns("<div>Hello World</div>");
			_environment.Setup(x => x.ExecuteWithBabel<JavaScriptWithSourceMap>(
				"ReactNET_transform_sourcemap",
				It.IsAny<string>(),
				It.IsAny<string>(), // Babel config
				"foo.jsx" // File name
			)).Returns(new JavaScriptWithSourceMap { Code = "React.DOM.div('Hello World')" });

			var result = _babel.TransformFile("foo.jsx");
			Assert.EndsWith("React.DOM.div('Hello World')", result);
		}

		[Fact]
		public void ShouldSaveTransformationResult()
		{
			_fileSystem.Setup(x => x.ReadAsString("foo.jsx")).Returns("<div>Hello World</div>");
			_environment.Setup(x => x.ExecuteWithBabel<JavaScriptWithSourceMap>(
				"ReactNET_transform_sourcemap",
				It.IsAny<string>(),
				It.IsAny<string>(), // Babel config
				"foo.jsx" // File name
			)).Returns(new JavaScriptWithSourceMap { Code = "React.DOM.div('Hello World')" });

			string result = null;
			_fileSystem.Setup(x => x.WriteAsString("foo.generated.js", It.IsAny<string>())).Callback(
				(string filename, string contents) => result = contents
			);

			var resultFilename = _babel.TransformAndSaveFile("foo.jsx");
			Assert.Equal("foo.generated.js", resultFilename);
			Assert.EndsWith("React.DOM.div('Hello World')", result);
		}

		[Fact]
		public void ShouldSkipTransformationIfCacheIsValid()
		{
			_fileSystem.Setup(x => x.ReadAsString("foo.jsx")).Returns("<div>Hello World</div>");
			_fileSystem.Setup(x => x.FileExists(It.IsAny<string>())).Returns(true);
			_fileCacheHash.Setup(x => x.ValidateHash(It.IsAny<string>(), It.IsAny<string>())).Returns(true);
			_environment.Setup(x => x.ExecuteWithBabel<JavaScriptWithSourceMap>(
				"ReactNET_transform_sourcemap",
				It.IsAny<string>(),
				It.IsAny<string>(), // Babel config
				"foo.jsx" // File name
			)).Returns(new JavaScriptWithSourceMap { Code = "React.DOM.div('Hello World')" });

			string result = null;
			_fileSystem.Setup(x => x.WriteAsString("foo.generated.js", It.IsAny<string>())).Callback(
				(string filename, string contents) => result = contents
			);

			var resultFilename = _babel.TransformAndSaveFile("foo.jsx");
			Assert.Equal("foo.generated.js", resultFilename);
            // There should be no result. Cached result should have been used.
            Assert.Null(result);
		}

		private void SetUpEmptyCache()
		{
			_cache.Setup(x => x.Get<JavaScriptWithSourceMap>("JSX_v3_foo.jsx", null)).Returns((JavaScriptWithSourceMap)null);
		}
	}
}
