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
using NUnit.Framework;
using React.Exceptions;

namespace React.Tests.Core
{
	[TestFixture]
	public class BabelTransformerTests
	{
		private Mock<IReactEnvironment> _environment;
		private Mock<ICache> _cache;
		private Mock<IFileSystem> _fileSystem;
		private Mock<IFileCacheHash> _fileCacheHash;
		private Babel _babel;

		[SetUp]
		public void SetUp()
		{
			_environment = new Mock<IReactEnvironment>();

			_cache = new Mock<ICache>();
			_fileSystem = new Mock<IFileSystem>();
			_fileSystem.Setup(x => x.MapPath(It.IsAny<string>())).Returns<string>(x => x);

			_fileCacheHash = new Mock<IFileCacheHash>();

			_babel = new Babel(
				_environment.Object,
				_cache.Object,
				_fileSystem.Object,
				_fileCacheHash.Object,
				ReactSiteConfiguration.Configuration
			);
		}

		[Test]
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

		[Test]
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

		[Test]
		public void ShouldUseCacheProvider()
		{
			_cache.Setup(x => x.Get<JavaScriptWithSourceMap>("JSX_v3_foo.jsx", null)).Returns(new JavaScriptWithSourceMap
			{
				Code = "/* cached */"
			});

			var result = _babel.TransformFile("foo.jsx");
			Assert.AreEqual("/* cached */", result);
		}

		[Test]
		public void ShouldUseFileSystemCacheIfHashValid()
		{
			SetUpEmptyCache();
			_fileSystem.Setup(x => x.FileExists("foo.generated.js")).Returns(true);
			_fileSystem.Setup(x => x.ReadAsString("foo.generated.js")).Returns("/* filesystem cached */");
			_fileCacheHash.Setup(x => x.ValidateHash(It.IsAny<string>(), It.IsAny<string>())).Returns(true);

			var result = _babel.TransformFile("foo.jsx");
			Assert.AreEqual("/* filesystem cached */", result);
		}

		[Test]
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
			StringAssert.EndsWith("React.DOM.div('Hello World')", result);
		}

		[Test]
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
			StringAssert.EndsWith("React.DOM.div('Hello World')", result);
		}

		[Test]
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
			Assert.AreEqual("foo.generated.js", resultFilename);
			StringAssert.EndsWith("React.DOM.div('Hello World')", result);
		}

		private void SetUpEmptyCache()
		{
			_cache.Setup(x => x.Get<JavaScriptWithSourceMap>("JSX_v3_foo.jsx", null)).Returns((JavaScriptWithSourceMap)null);
		}
	}
}
