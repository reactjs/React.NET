/*
 *  Copyright (c) 2014-Present, Facebook, Inc.
 *  All rights reserved.
 *
 *  This source code is licensed under the BSD-style license found in the
 *  LICENSE file in the root directory of this source tree. An additional grant 
 *  of patent rights can be found in the PATENTS file in the same directory.
 */

using System.Collections.Generic;
using System.Threading;
using JavaScriptEngineSwitcher.Core;
using Moq;
using NUnit.Framework;
using React.Exceptions;

namespace React.Tests.Core
{
	[TestFixture]
	public class JavaScriptEngineFactoryTest
	{
		private JavaScriptEngineFactory CreateFactory()
		{
			var config = new Mock<IReactSiteConfiguration>();
			config.Setup(x => x.ScriptsWithoutTransform).Returns(new List<string>());
			config.Setup(x => x.LoadReact).Returns(true);
			var fileSystem = new Mock<IFileSystem>();
			var registration = new JavaScriptEngineFactory.Registration
			{
				Factory = () =>
				{
					var mockJsEngine = new Mock<IJsEngine>();
					mockJsEngine.Setup(x => x.Evaluate<int>("1 + 1")).Returns(2);
					return mockJsEngine.Object;
				},
				Priority = 1
			};
			return new JavaScriptEngineFactory(new[] { registration }, config.Object, fileSystem.Object);
		}

		[Test]
		public void ShouldReturnSameEngine()
		{
			var factory = CreateFactory();
			var engine1 = factory.GetEngineForCurrentThread();
			var engine2 = factory.GetEngineForCurrentThread();
			
			Assert.AreEqual(engine1, engine2);
			factory.DisposeEngineForCurrentThread();
		}

		[Test]
		public void ShouldReturnNewEngineAfterDisposing()
		{
			var factory = CreateFactory();
			var engine1 = factory.GetEngineForCurrentThread();
			factory.DisposeEngineForCurrentThread();
			var engine2 = factory.GetEngineForCurrentThread();
			factory.DisposeEngineForCurrentThread();

			Assert.AreNotEqual(engine1, engine2);
		}

		[Test]
		public void ShouldCreateNewEngineForNewThread()
		{
			var factory = CreateFactory();
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
			Assert.AreNotEqual(engine1, engine2);
			// Same thread should share same engine
			Assert.AreEqual(engine1, engine3);
			factory.DisposeEngineForCurrentThread();
		}

		[Test]
		public void ShouldLoadFilesThatDoNotRequireTransform()
		{
			var jsEngine = new Mock<IJsEngine>();
			jsEngine.Setup(x => x.Evaluate<int>("1 + 1")).Returns(2);

			var config = new Mock<IReactSiteConfiguration>();
			config.Setup(x => x.ScriptsWithoutTransform).Returns(new List<string> { "First.js", "Second.js" });
			config.Setup(x => x.LoadReact).Returns(true);

			var fileSystem = new Mock<IFileSystem>();
			fileSystem.Setup(x => x.ReadAsString(It.IsAny<string>())).Returns<string>(path => "CONTENTS_" + path);

			var registration = new JavaScriptEngineFactory.Registration
			{
				Factory = () => jsEngine.Object,
				Priority = 1
			};
			var factory = new JavaScriptEngineFactory(new[] { registration }, config.Object, fileSystem.Object);

			factory.GetEngineForCurrentThread();

			jsEngine.Verify(x => x.Execute("CONTENTS_First.js"));
			jsEngine.Verify(x => x.Execute("CONTENTS_Second.js"));
		}

		[Test]
		public void ShouldHandleLoadingExternalReactVersion()
		{
			var jsEngine = new Mock<IJsEngine>();
			jsEngine.Setup(x => x.Evaluate<int>("1 + 1")).Returns(2);
			jsEngine.Setup(x => x.CallFunction<bool>("ReactNET_initReact")).Returns(true);
			var config = new Mock<IReactSiteConfiguration>();
			config.Setup(x => x.ScriptsWithoutTransform).Returns(new List<string>());
			config.Setup(x => x.LoadReact).Returns(false);
			var fileSystem = new Mock<IFileSystem>();
			var registration = new JavaScriptEngineFactory.Registration
			{
				Factory = () => jsEngine.Object,
				Priority = 1
			};
			var factory = new JavaScriptEngineFactory(new[] { registration }, config.Object, fileSystem.Object);

			factory.GetEngineForCurrentThread();

			jsEngine.Verify(x => x.CallFunction<bool>("ReactNET_initReact"));
		}

		[Test]
		public void ShouldThrowIfReactVersionNotLoaded()
		{
			var jsEngine = new Mock<IJsEngine>();
			jsEngine.Setup(x => x.Evaluate<int>("1 + 1")).Returns(2);
			jsEngine.Setup(x => x.CallFunction<bool>("ReactNET_initReact")).Returns(false);
			var config = new Mock<IReactSiteConfiguration>();
			config.Setup(x => x.ScriptsWithoutTransform).Returns(new List<string>());
			config.Setup(x => x.LoadReact).Returns(false);
			var fileSystem = new Mock<IFileSystem>();
			var registration = new JavaScriptEngineFactory.Registration
			{
				Factory = () => jsEngine.Object,
				Priority = 1
			};
			var factory = new JavaScriptEngineFactory(new[] { registration }, config.Object, fileSystem.Object);

			Assert.Throws<ReactNotInitialisedException>(() =>
			{
				factory.GetEngineForCurrentThread();
			});
		}

		[Test]
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
			var registration = new JavaScriptEngineFactory.Registration
			{
				Factory = () => jsEngine.Object,
				Priority = 1
			};
			var factory = new JavaScriptEngineFactory(new[] { registration }, config.Object, fileSystem.Object);

			var ex = Assert.Throws<ReactScriptLoadException>(() => factory.GetEngineForCurrentThread());
			Assert.AreEqual("Error while loading \"foo.js\": Fail\r\nLine: 42\r\nColumn: 911", ex.Message);
		}
	}
}
