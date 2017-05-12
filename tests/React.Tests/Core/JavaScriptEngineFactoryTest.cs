/*
 *  Copyright (c) 2014-Present, Facebook, Inc.
 *  All rights reserved.
 *
 *  This source code is licensed under the BSD-style license found in the
 *  LICENSE file in the root directory of this source tree. An additional grant 
 *  of patent rights can be found in the PATENTS file in the same directory.
 */

using System;
using System.Collections.Generic;
using System.Threading;
using JavaScriptEngineSwitcher.Core;
using Moq;
using Xunit;
using React.Exceptions;

namespace React.Tests.Core
{
	public class JavaScriptEngineFactoryTest
	{
		private JavaScriptEngineFactory CreateBasicFactory()
		{
			var config = new Mock<IReactSiteConfiguration>();
			config.Setup(x => x.ScriptsWithoutTransform).Returns(new List<string>());
			config.Setup(x => x.LoadReact).Returns(true);
			var fileSystem = new Mock<IFileSystem>();
			return CreateFactory(config, fileSystem, () =>
			{
				var mockJsEngine = new Mock<IJsEngine>();
				mockJsEngine.Setup(x => x.Evaluate<int>("1 + 1")).Returns(2);
				return mockJsEngine.Object;
			});
		}

		private JavaScriptEngineFactory CreateFactory(
			Mock<IReactSiteConfiguration> config, 
			Mock<IFileSystem> fileSystem,
			Func<IJsEngine> innerEngineFactory
		)
		{
			var engineFactory = new Mock<IJsEngineFactory>();
			engineFactory.Setup(x => x.EngineName).Returns("MockEngine");
			engineFactory.Setup(x => x.CreateEngine()).Returns(innerEngineFactory);

			// JsEngineSwitcher is a singleton :(
			var engineFactories = JsEngineSwitcher.Instance.EngineFactories;
			engineFactories.Clear();
			engineFactories.Add(engineFactory.Object);
			
			return new JavaScriptEngineFactory(JsEngineSwitcher.Instance, config.Object, fileSystem.Object);
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
		public void ShouldLoadFilesThatDoNotRequireTransform()
		{
			var jsEngine = new Mock<IJsEngine>();
			jsEngine.Setup(x => x.Evaluate<int>("1 + 1")).Returns(2);

			var config = new Mock<IReactSiteConfiguration>();
			config.Setup(x => x.ScriptsWithoutTransform).Returns(new List<string> { "First.js", "Second.js" });
			config.Setup(x => x.LoadReact).Returns(true);

			var fileSystem = new Mock<IFileSystem>();
			fileSystem.Setup(x => x.ReadAsString(It.IsAny<string>())).Returns<string>(path => "CONTENTS_" + path);

			var factory = CreateFactory(config, fileSystem, () => jsEngine.Object);

			factory.GetEngineForCurrentThread();

			jsEngine.Verify(x => x.Execute("CONTENTS_First.js"));
			jsEngine.Verify(x => x.Execute("CONTENTS_Second.js"));
		}

		[Fact]
		public void ShouldHandleLoadingExternalReactVersion()
		{
			var jsEngine = new Mock<IJsEngine>();
			jsEngine.Setup(x => x.Evaluate<int>("1 + 1")).Returns(2);
			jsEngine.Setup(x => x.CallFunction<bool>("ReactNET_initReact")).Returns(true);
			var config = new Mock<IReactSiteConfiguration>();
			config.Setup(x => x.ScriptsWithoutTransform).Returns(new List<string>());
			config.Setup(x => x.LoadReact).Returns(false);
			var fileSystem = new Mock<IFileSystem>();
			var factory = CreateFactory(config, fileSystem, () => jsEngine.Object);

			factory.GetEngineForCurrentThread();

			jsEngine.Verify(x => x.CallFunction<bool>("ReactNET_initReact"));
		}

		[Fact]
		public void ShouldThrowIfReactVersionNotLoaded()
		{
			var jsEngine = new Mock<IJsEngine>();
			jsEngine.Setup(x => x.Evaluate<int>("1 + 1")).Returns(2);
			jsEngine.Setup(x => x.CallFunction<bool>("ReactNET_initReact")).Returns(false);
			var config = new Mock<IReactSiteConfiguration>();
			config.Setup(x => x.ScriptsWithoutTransform).Returns(new List<string>());
			config.Setup(x => x.LoadReact).Returns(false);
			var fileSystem = new Mock<IFileSystem>();
			var factory = CreateFactory(config, fileSystem, () => jsEngine.Object);

			Assert.Throws<ReactNotInitialisedException>(() =>
			{
				factory.GetEngineForCurrentThread();
			});
		}

		[Fact]
		public void ShouldCatchErrorsWhileLoadingScripts()
		{
			var config = new Mock<IReactSiteConfiguration>();
			config.Setup(x => x.ScriptsWithoutTransform).Returns(new List<string> {"foo.js"});
			config.Setup(x => x.LoadReact).Returns(true);
			var fileSystem = new Mock<IFileSystem>();
			fileSystem.Setup(x => x.ReadAsString("foo.js")).Returns("FAIL PLZ");

			var jsEngine = new Mock<IJsEngine>();
			jsEngine.Setup(x => x.Evaluate<int>("1 + 1")).Returns(2);
			jsEngine.Setup(x => x.Execute("FAIL PLZ")).Throws(new JsRuntimeException("Fail")
			{
				LineNumber = 42,
				ColumnNumber = 911,
			});
			var factory = CreateFactory(config, fileSystem, () => jsEngine.Object);

			var ex = Assert.Throws<ReactScriptLoadException>(() => factory.GetEngineForCurrentThread());
			Assert.Equal("Error while loading \"foo.js\": Fail\r\nLine: 42\r\nColumn: 911", ex.Message);
		}
	}
}
