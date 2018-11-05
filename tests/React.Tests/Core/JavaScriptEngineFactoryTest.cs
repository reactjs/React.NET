/*
 * Copyright (c) Facebook, Inc. and its affiliates.
 *
 * This source code is licensed under the MIT license found in the
 * LICENSE file in the root directory of this source tree.
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading;
using JavaScriptEngineSwitcher.Core;
using Moq;
using React.Exceptions;
using Xunit;

namespace React.Tests.Core
{
	public class JavaScriptEngineFactoryTest
	{
		private JavaScriptEngineFactory CreateBasicFactory()
		{
			var config = new Mock<IReactSiteConfiguration>();
			config.Setup(x => x.ScriptsWithoutTransform).Returns(new List<string>());
			config.Setup(x => x.LoadReact).Returns(true);
			var cache = new Mock<ICache>();
			var fileSystem = new Mock<IFileSystem>();
			return CreateFactory(config, cache, fileSystem, () =>
			{
				var mockJsEngine = new Mock<IJsEngine>();
				mockJsEngine.Setup(x => x.Evaluate<int>("1 + 1")).Returns(2);
				return mockJsEngine.Object;
			});
		}

		private JavaScriptEngineFactory CreateFactory(
			Mock<IReactSiteConfiguration> config,
			Mock<ICache> cache,
			Mock<IFileSystem> fileSystem,
			Func<IJsEngine> innerEngineFactory
		)
		{
			return CreateFactory(config.Object, cache.Object, fileSystem.Object, innerEngineFactory);
		}

		private JavaScriptEngineFactory CreateFactory(
			IReactSiteConfiguration config,
			ICache cache,
			IFileSystem fileSystem,
			Func<IJsEngine> innerEngineFactory
		)
		{
			var engineFactory = new Mock<IJsEngineFactory>();
			engineFactory.Setup(x => x.EngineName).Returns("MockEngine");
			engineFactory.Setup(x => x.CreateEngine()).Returns(innerEngineFactory);

			var engineSwitcher = new JsEngineSwitcher(
				new JsEngineFactoryCollection { engineFactory.Object },
				string.Empty
			);

			return new JavaScriptEngineFactory(engineSwitcher, config, cache, fileSystem);
		}

		[Fact]
		public void ShouldReturnSameEngine()
		{
			var factory = CreateBasicFactory();
			var engine1 = factory.GetEngineForCurrentThread();
			var engine2 = factory.GetEngineForCurrentThread();

			Assert.Equal(engine1, engine2);
			factory.DisposeEngineForCurrentThread();
		}

		[Fact]
		public void ShouldReturnNewEngineAfterDisposing()
		{
			var factory = CreateBasicFactory();
			var engine1 = factory.GetEngineForCurrentThread();
			factory.DisposeEngineForCurrentThread();
			var engine2 = factory.GetEngineForCurrentThread();
			factory.DisposeEngineForCurrentThread();

			Assert.NotEqual(engine1, engine2);
		}

		[Fact]
		public void ShouldCreateNewEngineForNewThread()
		{
			var factory = CreateBasicFactory();
			var engine1 = factory.GetEngineForCurrentThread();

			IJsEngine engine2 = null;
			var thread = new Thread(() =>
			{
				engine2 = factory.GetEngineForCurrentThread();
				// Need to ensure engine is disposed in same thread as it was created in
				factory.DisposeEngineForCurrentThread();
			});
			thread.Start();
			thread.Join();

			var engine3 = factory.GetEngineForCurrentThread();

			// Different threads should have different engines
			Assert.NotEqual(engine1, engine2);
			// Same thread should share same engine
			Assert.Equal(engine1, engine3);
			factory.DisposeEngineForCurrentThread();
		}

		[Fact]
		public void ShouldLoadResourcesWithoutPrecompilation()
		{
			var reactCoreAssembly = typeof(JavaScriptEngineFactory).GetTypeInfo().Assembly;

			var jsEngine = new Mock<IJsEngine>();
			jsEngine.Setup(x => x.SupportsScriptPrecompilation).Returns(true);
			jsEngine.Setup(x => x.Evaluate<int>("1 + 1")).Returns(2);

			var config = new Mock<IReactSiteConfiguration>();
			config.Setup(x => x.AllowJavaScriptPrecompilation).Returns(false);
			config.Setup(x => x.LoadReact).Returns(true);

			var cache = new Mock<ICache>();

			var fileSystem = new Mock<IFileSystem>();

			var factory = CreateFactory(config, cache, fileSystem, () => jsEngine.Object);

			factory.GetEngineForCurrentThread();

			jsEngine.Verify(x => x.ExecuteResource("React.Core.Resources.shims.js", reactCoreAssembly));
			jsEngine.Verify(x => x.ExecuteResource("React.Core.Resources.react.generated.min.js", reactCoreAssembly));
		}

		[Fact]
		public void ShouldLoadResourcesWithPrecompilationAndWithoutCache()
		{
			var reactCoreAssembly = typeof(JavaScriptEngineFactory).GetTypeInfo().Assembly;

			var jsEngine = new Mock<IJsEngine>();
			jsEngine.Setup(x => x.SupportsScriptPrecompilation).Returns(true);
			jsEngine.Setup(x => x.Evaluate<int>("1 + 1")).Returns(2);

			var config = new Mock<IReactSiteConfiguration>();
			config.Setup(x => x.AllowJavaScriptPrecompilation).Returns(true);
			config.Setup(x => x.LoadReact).Returns(true);

			var cache = new NullCache();

			var fileSystem = new Mock<IFileSystem>();

			var factory = CreateFactory(config.Object, cache, fileSystem.Object, () => jsEngine.Object);

			factory.GetEngineForCurrentThread();

			jsEngine.Verify(x => x.ExecuteResource("React.Core.Resources.shims.js", reactCoreAssembly));
			jsEngine.Verify(x => x.ExecuteResource("React.Core.Resources.react.generated.min.js", reactCoreAssembly));
		}

		[Fact]
		public void ShouldLoadResourcesWithPrecompilationAndEmptyCache()
		{
			var reactCoreAssembly = typeof(JavaScriptEngineFactory).GetTypeInfo().Assembly;
			var shimsPrecompiledScript = new Mock<IPrecompiledScript>().Object;
			var reactPrecompiledScript = new Mock<IPrecompiledScript>().Object;

			var jsEngine = new Mock<IJsEngine>();
			jsEngine.Setup(x => x.SupportsScriptPrecompilation).Returns(true);
			jsEngine.Setup(x => x.Evaluate<int>("1 + 1")).Returns(2);
			jsEngine
				.Setup(x => x.PrecompileResource("React.Core.Resources.shims.js", reactCoreAssembly))
				.Returns(shimsPrecompiledScript);
			jsEngine
				.Setup(x => x.PrecompileResource("React.Core.Resources.react.generated.min.js", reactCoreAssembly))
				.Returns(reactPrecompiledScript);

			var config = new Mock<IReactSiteConfiguration>();
			config.Setup(x => x.AllowJavaScriptPrecompilation).Returns(true);
			config.Setup(x => x.LoadReact).Returns(true);

			var cache = new Mock<ICache>();

			var fileSystem = new Mock<IFileSystem>();

			var factory = CreateFactory(config, cache, fileSystem, () => jsEngine.Object);

			factory.GetEngineForCurrentThread();

			jsEngine.Verify(x => x.Execute(shimsPrecompiledScript));
			jsEngine.Verify(x => x.Execute(reactPrecompiledScript));
		}

		[Fact]
		public void ShouldLoadResourcesWithPrecompilationAndNotEmptyCache()
		{
			var jsEngine = new Mock<IJsEngine>();
			jsEngine.Setup(x => x.SupportsScriptPrecompilation).Returns(true);
			jsEngine.Setup(x => x.Evaluate<int>("1 + 1")).Returns(2);

			var config = new Mock<IReactSiteConfiguration>();
			config.Setup(x => x.AllowJavaScriptPrecompilation).Returns(true);
			config.Setup(x => x.LoadReact).Returns(true);

			var shimsPrecompiledScript = new Mock<IPrecompiledScript>().Object;
			var reactPrecompiledScript = new Mock<IPrecompiledScript>().Object;

			var cache = new Mock<ICache>();
			cache
				.Setup(x => x.Get<IPrecompiledScript>("PRECOMPILED_JS_RESOURCE_React.Core.Resources.shims.js", null))
				.Returns(shimsPrecompiledScript);
			cache
				.Setup(x => x.Get<IPrecompiledScript>("PRECOMPILED_JS_RESOURCE_React.Core.Resources.react.generated.min.js", null))
				.Returns(reactPrecompiledScript);

			var fileSystem = new Mock<IFileSystem>();

			var factory = CreateFactory(config, cache, fileSystem, () => jsEngine.Object);

			factory.GetEngineForCurrentThread();

			jsEngine.Verify(x => x.Execute(shimsPrecompiledScript));
			jsEngine.Verify(x => x.Execute(reactPrecompiledScript));
		}

		[Fact]
		public void ShouldLoadFilesThatDoNotRequireTransformWithoutPrecompilation()
		{
			var jsEngine = new Mock<IJsEngine>();
			jsEngine.Setup(x => x.SupportsScriptPrecompilation).Returns(true);
			jsEngine.Setup(x => x.Evaluate<int>("1 + 1")).Returns(2);

			var config = new Mock<IReactSiteConfiguration>();
			config.Setup(x => x.ScriptsWithoutTransform).Returns(new List<string> { "First.js", "Second.js" });
			config.Setup(x => x.AllowJavaScriptPrecompilation).Returns(false);
			config.Setup(x => x.LoadReact).Returns(true);

			var cache = new Mock<ICache>();

			var fileSystem = new Mock<IFileSystem>();
			fileSystem.Setup(x => x.ReadAsString(It.IsAny<string>())).Returns<string>(path => "CONTENTS_" + path);

			var factory = CreateFactory(config, cache, fileSystem, () => jsEngine.Object);

			factory.GetEngineForCurrentThread();

			jsEngine.Verify(x => x.Execute("CONTENTS_First.js", "First.js"));
			jsEngine.Verify(x => x.Execute("CONTENTS_Second.js", "Second.js"));
		}

		[Fact]
		public void ShouldLoadFilesThatDoNotRequireTransformWithPrecompilationAndWithoutCache()
		{
			var jsEngine = new Mock<IJsEngine>();
			jsEngine.Setup(x => x.SupportsScriptPrecompilation).Returns(true);
			jsEngine.Setup(x => x.Evaluate<int>("1 + 1")).Returns(2);

			var config = new Mock<IReactSiteConfiguration>();
			config.Setup(x => x.ScriptsWithoutTransform).Returns(new List<string> { "First.js", "Second.js" });
			config.Setup(x => x.AllowJavaScriptPrecompilation).Returns(true);
			config.Setup(x => x.LoadReact).Returns(true);

			var cache = new NullCache();

			var fileSystem = new Mock<IFileSystem>();
			fileSystem.Setup(x => x.ReadAsString(It.IsAny<string>())).Returns<string>(path => "CONTENTS_" + path);

			var factory = CreateFactory(config.Object, cache, fileSystem.Object, () => jsEngine.Object);

			factory.GetEngineForCurrentThread();

			jsEngine.Verify(x => x.Execute("CONTENTS_First.js", "First.js"));
			jsEngine.Verify(x => x.Execute("CONTENTS_Second.js", "Second.js"));
		}

		[Fact]
		public void ShouldLoadFilesThatDoNotRequireTransformWithPrecompilationAndEmptyCache()
		{
			var firstPrecompiledScript = new Mock<IPrecompiledScript>().Object;
			var secondPrecompiledScript = new Mock<IPrecompiledScript>().Object;

			var jsEngine = new Mock<IJsEngine>();
			jsEngine.Setup(x => x.SupportsScriptPrecompilation).Returns(true);
			jsEngine.Setup(x => x.Evaluate<int>("1 + 1")).Returns(2);
			jsEngine.Setup(x => x.Precompile("CONTENTS_First.js", "First.js")).Returns(firstPrecompiledScript);
			jsEngine.Setup(x => x.Precompile("CONTENTS_Second.js", "Second.js")).Returns(secondPrecompiledScript);

			var config = new Mock<IReactSiteConfiguration>();
			config.Setup(x => x.ScriptsWithoutTransform).Returns(new List<string> { "First.js", "Second.js" });
			config.Setup(x => x.AllowJavaScriptPrecompilation).Returns(true);
			config.Setup(x => x.LoadReact).Returns(true);

			var cache = new Mock<ICache>();

			var fileSystem = new Mock<IFileSystem>();
			fileSystem.Setup(x => x.ReadAsString(It.IsAny<string>())).Returns<string>(path => "CONTENTS_" + path);

			var factory = CreateFactory(config, cache, fileSystem, () => jsEngine.Object);

			factory.GetEngineForCurrentThread();

			jsEngine.Verify(x => x.Execute(firstPrecompiledScript));
			jsEngine.Verify(x => x.Execute(secondPrecompiledScript));
		}

		[Fact]
		public void ShouldLoadFilesThatDoNotRequireTransformWithPrecompilationAndNotEmptyCache()
		{
			var jsEngine = new Mock<IJsEngine>();
			jsEngine.Setup(x => x.SupportsScriptPrecompilation).Returns(true);
			jsEngine.Setup(x => x.Evaluate<int>("1 + 1")).Returns(2);

			var config = new Mock<IReactSiteConfiguration>();
			config.Setup(x => x.ScriptsWithoutTransform).Returns(new List<string> { "First.js", "Second.js" });
			config.Setup(x => x.AllowJavaScriptPrecompilation).Returns(true);
			config.Setup(x => x.LoadReact).Returns(true);

			var firstPrecompiledScript = new Mock<IPrecompiledScript>().Object;
			var secondPrecompiledScript = new Mock<IPrecompiledScript>().Object;

			var cache = new Mock<ICache>();
			cache
				.Setup(x => x.Get<IPrecompiledScript>("PRECOMPILED_JS_FILE_First.js", null))
				.Returns(firstPrecompiledScript);
			cache
				.Setup(x => x.Get<IPrecompiledScript>("PRECOMPILED_JS_FILE_Second.js", null))
				.Returns(secondPrecompiledScript);

			var fileSystem = new Mock<IFileSystem>();
			fileSystem.Setup(x => x.ReadAsString(It.IsAny<string>())).Returns<string>(path => "CONTENTS_" + path);

			var factory = CreateFactory(config, cache, fileSystem, () => jsEngine.Object);

			factory.GetEngineForCurrentThread();

			jsEngine.Verify(x => x.Execute(firstPrecompiledScript));
			jsEngine.Verify(x => x.Execute(secondPrecompiledScript));
		}

		[Fact]
		public void ShouldHandleLoadingExternalReactVersion()
		{
			var jsEngine = new Mock<IJsEngine>();
			jsEngine.Setup(x => x.Evaluate<int>("1 + 1")).Returns(2);
			jsEngine.Setup(x => x.CallFunction<string>("ReactNET_initReact")).Returns(string.Empty);
			var config = new Mock<IReactSiteConfiguration>();
			config.Setup(x => x.ScriptsWithoutTransform).Returns(new List<string>());
			config.Setup(x => x.LoadReact).Returns(false);
			var cache = new Mock<ICache>();
			var fileSystem = new Mock<IFileSystem>();
			var factory = CreateFactory(config, cache, fileSystem, () => jsEngine.Object);

			factory.GetEngineForCurrentThread();

			jsEngine.Verify(x => x.CallFunction<string>("ReactNET_initReact"));
		}

		[Fact]
		public void ShouldThrowIfReactVersionNotLoaded()
		{
			var jsEngine = new Mock<IJsEngine>();
			jsEngine.Setup(x => x.Evaluate<int>("1 + 1")).Returns(2);
			jsEngine.Setup(x => x.CallFunction<string>("ReactNET_initReact")).Returns("React");
			var config = new Mock<IReactSiteConfiguration>();
			config.Setup(x => x.ScriptsWithoutTransform).Returns(new List<string>());
			config.Setup(x => x.LoadReact).Returns(false);
			var cache = new Mock<ICache>();
			var fileSystem = new Mock<IFileSystem>();
			var factory = CreateFactory(config, cache, fileSystem, () => jsEngine.Object);

			Assert.Throws<ReactNotInitialisedException>(() =>
			{
				factory.GetEngineForCurrentThread();
			});
		}

		[Fact]
		public void FileLockExceptionShouldBeWrapped()
		{
			var config = new Mock<IReactSiteConfiguration>();
			config.Setup(x => x.ScriptsWithoutTransform).Returns(new List<string> { "foo.js" });
			config.Setup(x => x.LoadReact).Returns(false);
			var cache = new Mock<ICache>();
			var fileSystem = new Mock<IFileSystem>();
			fileSystem.Setup(x => x.ReadAsString("foo.js")).Throws(new IOException("File was locked"));

			var jsEngine = new Mock<IJsEngine>();
			jsEngine.Setup(x => x.Evaluate<int>("1 + 1")).Returns(2);
			jsEngine.Setup(x => x.Execute("Test"));
			var factory = CreateFactory(config, cache, fileSystem, () => jsEngine.Object);

			var ex = Assert.Throws<ReactScriptLoadException>(() => factory.GetEngineForCurrentThread());
			Assert.Equal("File was locked", ex.Message);
		}

		[Fact]
		public void ShouldThrowScriptErrorIfReactFails()
		{
			var config = new Mock<IReactSiteConfiguration>();
			config.Setup(x => x.ScriptsWithoutTransform).Returns(new List<string> {"foo.js"});
			config.Setup(x => x.LoadReact).Returns(false);
			var cache = new Mock<ICache>();
			var fileSystem = new Mock<IFileSystem>();
			fileSystem.Setup(x => x.ReadAsString("foo.js")).Returns("FAIL PLZ");

			var jsEngine = new Mock<IJsEngine>();
			jsEngine.Setup(x => x.Evaluate<int>("1 + 1")).Returns(2);
			jsEngine.Setup(x => x.Execute("FAIL PLZ", "foo.js")).Throws(new JsRuntimeException("Fail")
			{
				LineNumber = 42,
				ColumnNumber = 911,
			});
			var factory = CreateFactory(config, cache, fileSystem, () => jsEngine.Object);

			var ex = Assert.Throws<ReactScriptLoadException>(() => factory.GetEngineForCurrentThread());
			Assert.Equal("Error while loading \"foo.js\": Fail\r\nLine: 42\r\nColumn: 911", ex.Message);
		}

		[Fact]
		public void ShouldCatchErrorsWhileLoadingScripts()
		{
			var config = new Mock<IReactSiteConfiguration>();
			config.Setup(x => x.ScriptsWithoutTransform).Returns(new List<string> {"foo.js"});
			config.Setup(x => x.LoadReact).Returns(true);
			var cache = new Mock<ICache>();
			var fileSystem = new Mock<IFileSystem>();
			fileSystem.Setup(x => x.ReadAsString("foo.js")).Returns("FAIL PLZ");

			var jsEngine = new Mock<IJsEngine>();
			jsEngine.Setup(x => x.Evaluate<int>("1 + 1")).Returns(2);
			jsEngine.Setup(x => x.Execute("FAIL PLZ", "foo.js")).Throws(new JsRuntimeException("Fail")
			{
				LineNumber = 42,
				ColumnNumber = 911,
			});
			var factory = CreateFactory(config, cache, fileSystem, () => jsEngine.Object);

			var ex = Assert.Throws<ReactScriptLoadException>(() => factory.GetEngineForCurrentThread());
			Assert.Equal("Error while loading \"foo.js\": Fail\r\nLine: 42\r\nColumn: 911", ex.Message);
		}

		[Fact]
		public void ShouldReturnDefaultEngine()
		{
			const string someEngineName = "SomeEngine";
			const string defaultEngineName = "DefaultEngine";
			const string someOtherEngineName = "SomeOtherEngine";

			var someEngineFactory = new Mock<IJsEngineFactory>();
			someEngineFactory.Setup(x => x.EngineName).Returns(someEngineName);
			someEngineFactory.Setup(x => x.CreateEngine()).Returns(() =>
			{
				var someEngine = new Mock<IJsEngine>();
				someEngine.Setup(x => x.Name).Returns(someEngineName);
				return someEngine.Object;
			});

			var defaultEngineFactory = new Mock<IJsEngineFactory>();
			defaultEngineFactory.Setup(x => x.EngineName).Returns(defaultEngineName);
			defaultEngineFactory.Setup(x => x.CreateEngine()).Returns(() =>
			{
				var defaultEngine = new Mock<IJsEngine>();
				defaultEngine.Setup(x => x.Name).Returns(defaultEngineName);
				return defaultEngine.Object;
			});

			var someOtherEngineFactory = new Mock<IJsEngineFactory>();
			someOtherEngineFactory.Setup(x => x.EngineName).Returns(someOtherEngineName);
			someOtherEngineFactory.Setup(x => x.CreateEngine()).Returns(() =>
			{
				var someOtherEngine = new Mock<IJsEngine>();
				someOtherEngine.Setup(x => x.Name).Returns(someOtherEngineName);
				return someOtherEngine.Object;
			});

			var config = new Mock<IReactSiteConfiguration>();
			config.Setup(x => x.ScriptsWithoutTransform).Returns(new List<string>());
			config.Setup(x => x.LoadReact).Returns(true);

			var cache = new Mock<ICache>();
			var fileSystem = new Mock<IFileSystem>();
			var engineSwitcher = new JsEngineSwitcher(
				new JsEngineFactoryCollection
				{
					someEngineFactory.Object,
					defaultEngineFactory.Object,
					someOtherEngineFactory.Object
				},
				defaultEngineName
			);

			var factory = new JavaScriptEngineFactory(engineSwitcher, config.Object, cache.Object,
				fileSystem.Object);
			var engine = factory.GetEngineForCurrentThread();

			Assert.Equal(defaultEngineName, engine.Name);
		}

		[Fact]
		public void ShouldThrowIfDefaultEngineFactoryNotFound()
		{
			const string someEngineName = "SomeEngine";
			const string defaultEngineName = "DefaultEngine";
			const string someOtherEngineName = "SomeOtherEngine";

			var someEngineFactory = new Mock<IJsEngineFactory>();
			someEngineFactory.Setup(x => x.EngineName).Returns(someEngineName);
			someEngineFactory.Setup(x => x.CreateEngine()).Returns(() =>
			{
				var someEngine = new Mock<IJsEngine>();
				someEngine.Setup(x => x.Name).Returns(someEngineName);
				return someEngine.Object;
			});

			var someOtherEngineFactory = new Mock<IJsEngineFactory>();
			someOtherEngineFactory.Setup(x => x.EngineName).Returns(someOtherEngineName);
			someOtherEngineFactory.Setup(x => x.CreateEngine()).Returns(() =>
			{
				var someOtherEngine = new Mock<IJsEngine>();
				someOtherEngine.Setup(x => x.Name).Returns(someOtherEngineName);
				return someOtherEngine.Object;
			});

			var config = new Mock<IReactSiteConfiguration>();
			config.Setup(x => x.ScriptsWithoutTransform).Returns(new List<string>());
			config.Setup(x => x.LoadReact).Returns(true);

			var cache = new Mock<ICache>();
			var fileSystem = new Mock<IFileSystem>();
			var engineSwitcher = new JsEngineSwitcher(
				new JsEngineFactoryCollection
				{
					someEngineFactory.Object,
					someOtherEngineFactory.Object
				},
				defaultEngineName
			);

			Assert.Throws<ReactEngineNotFoundException>(() =>
			{
				var factory = new JavaScriptEngineFactory(engineSwitcher, config.Object, cache.Object,
					fileSystem.Object);
			});
		}

		[Fact]
		public void ShouldThrowIfNoEnginesRegistered()
		{
			var config = new Mock<IReactSiteConfiguration>();
			var cache = new Mock<ICache>();
			var fileSystem = new Mock<IFileSystem>();
			config.Setup(x => x.ScriptsWithoutTransform).Returns(new List<string>());
			config.Setup(x => x.LoadReact).Returns(true);

			var engineSwitcher = new JsEngineSwitcher(
				new JsEngineFactoryCollection(),
				string.Empty
			);

			var caughtException = Assert.Throws<ReactException>(() =>
			{
				new JavaScriptEngineFactory(engineSwitcher, config.Object, cache.Object, fileSystem.Object);
			});
			Assert.Contains("No JS engines were registered", caughtException.Message);
		}

		[Fact]
		public void ShouldThrowIfJsEngineFails()
		{
			const string defaultEngineName = "DefaultEngine";

			var defaultEngineFactory = new Mock<IJsEngineFactory>();
			defaultEngineFactory.Setup(x => x.EngineName).Returns(defaultEngineName);
			defaultEngineFactory.Setup(x => x.CreateEngine()).Throws(new JsEngineLoadException("This is a custom JS engine load error."));

			var cache = new Mock<ICache>();
			var config = new Mock<IReactSiteConfiguration>();
			config.Setup(x => x.ScriptsWithoutTransform).Returns(new List<string>());
			config.Setup(x => x.LoadReact).Returns(true);

			var fileSystem = new Mock<IFileSystem>();
			var engineSwitcher = new JsEngineSwitcher(
				new JsEngineFactoryCollection
				{
					defaultEngineFactory.Object,
				},
				string.Empty
			);

			var caughtException = Assert.Throws<ReactEngineNotFoundException>(() =>
			{
				var factory = new JavaScriptEngineFactory(engineSwitcher, config.Object, cache.Object,
					fileSystem.Object);
				factory.GetEngineForCurrentThread();
			});
			Assert.Contains("This is a custom JS engine load error", caughtException.Message);
		}
	}
}
