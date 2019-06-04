/*
 * Copyright (c) Facebook, Inc. and its affiliates.
 *
 * This source code is licensed under the MIT license found in the
 * LICENSE file in the root directory of this source tree.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using JavaScriptEngineSwitcher.Core;
using JSPool;
using Moq;
using Xunit;
using React.Exceptions;

namespace React.Tests.Core
{
	public class ReactEnvironmentTest
	{
		[Fact]
		public void ExecuteWithBabelWithNoNewThread()
		{
			var mocks = new Mocks();
			var environment = mocks.CreateReactEnvironment();

			environment.ExecuteWithBabel<int>("foo");
			mocks.Engine.Verify(x => x.CallFunction<int>("foo"), Times.Exactly(1));
		}
#if NET452 || NETCOREAPP2_0

		[Fact]
		public void ExecuteWithBabelWithNewThread()
		{
			var mocks = new Mocks();
			var environment = mocks.CreateReactEnvironment();
			// Fail the first time Evaluate is called, succeed the second
			// http://stackoverflow.com/a/7045636
			mocks.Engine.Setup(x => x.CallFunction<int>("foo"))
				.Callback(() => mocks.Engine.Setup(x => x.CallFunction<int>("foo")))
				.Throws(new Exception("Out of stack space"));

			environment.ExecuteWithBabel<int>("foo");
			mocks.EngineFactory.Verify(
				x => x.GetEngineForCurrentThread(),
				Times.Exactly(2),
				"Two engines should be created (initial thread and new thread)"
			);
			mocks.EngineFactory.Verify(
				x => x.DisposeEngineForCurrentThread(),
				Times.Exactly(1),
				"Inner engine should be disposed"
			);
		}
#endif

		[Fact]
		public void ExecuteWithBabelShouldBubbleExceptions()
		{
			var mocks = new Mocks();
			var environment = mocks.CreateReactEnvironment();
			// Always fail
			mocks.Engine.Setup(x => x.CallFunction<int>("foobar"))
				.Throws(new Exception("Something bad happened :("));

			Assert.Throws<Exception>(() =>
			{
				environment.ExecuteWithBabel<int>("foobar");
			});
		}

		[Fact]
		public void ExecuteWithBabelShouldThrowIfBabelDisabled()
		{
			var mocks = new Mocks();
			mocks.Config.Setup(x => x.LoadBabel).Returns(false);
			var environment = mocks.CreateReactEnvironment();

			Assert.Throws<BabelNotLoadedException>(() =>
			{
				environment.ExecuteWithBabel<string>("foobar");
			});
		}

		[Fact]
		public void UsesProvidedContainerId()
		{
			var mocks = new Mocks();
			var environment = mocks.CreateReactEnvironment();
			mocks.Config.Setup(x => x.Scripts).Returns(new List<string>());
			mocks.ReactIdGenerator.Setup(x => x.Generate()).Returns("react_customId");

			var component1 = environment.CreateComponent("ComponentName", new { }, "foo");
			var component2 = environment.CreateComponent("ComponentName", new { });
			Assert.Equal("foo", component1.ContainerId);
			Assert.Equal("react_customId", component2.ContainerId);
		}

		[Fact]
		public void ReturnsEngineToPool()
		{
			var mocks = new Mocks();
			var environment = mocks.CreateReactEnvironment();
			mocks.Config.Setup(x => x.ReuseJavaScriptEngines).Returns(true);

			environment.CreateComponent("ComponentName", new { });
			mocks.EngineFactory.Verify(x => x.GetEngine(), Times.Once);
			environment.ReturnEngineToPool();

			environment.CreateComponent("ComponentName", new { });
			mocks.EngineFactory.Verify(x => x.GetEngine(), Times.AtLeast(2));
		}

		[Fact]
		public void CreatesIReactComponent()
		{
			var mocks = new Mocks();
			var environment = mocks.CreateReactEnvironment();

			var component = new Mock<IReactComponent>();

			environment.CreateComponent(component.Object);

			// A single nameless component was successfully added!
			Assert.Equal(";" + Environment.NewLine, environment.GetInitJavaScript());
		}

		[Fact]
		public void ServerSideOnlyComponentRendersNoJavaScript()
		{
			var mocks = new Mocks();
			var environment = mocks.CreateReactEnvironment();

			environment.CreateComponent("HelloWorld", new { name = "Daniel" }, serverOnly: true);

			Assert.Equal(string.Empty, environment.GetInitJavaScript());
		}

		[Theory]
		[InlineData(false, 0)]
		[InlineData(true, 1)]
		public void SSRInitSkippedIfNoComponents(bool renderComponent, int ssrTimes)
		{
			var mocks = new Mocks();
			var environment = mocks.CreateReactEnvironment();

			if (renderComponent)
			{
				environment.CreateComponent("HelloWorld", new { name = "Daniel" }, clientOnly: true).RenderHtml();
			}

			environment.GetInitJavaScript();

			mocks.Engine.Verify(x => x.Evaluate<string>("console.getCalls()"), Times.Exactly(ssrTimes));
		}

		public class Mocks
		{
			public Mock<PooledJsEngine> Engine { get; private set; }
			public Mock<IJavaScriptEngineFactory> EngineFactory { get; private set; }
			public Mock<IReactSiteConfiguration> Config { get; private set; }
			public Mock<ICache> Cache { get; private set; }
			public Mock<IFileSystem> FileSystem { get; private set; }
			public Mock<IFileCacheHash> FileCacheHash { get; private set; }
			public Mock<IReactIdGenerator> ReactIdGenerator { get; private set; }
			public Mocks()
			{
				Engine = new Mock<PooledJsEngine>();
				EngineFactory = new Mock<IJavaScriptEngineFactory>();
				Config = new Mock<IReactSiteConfiguration>();
				Cache = new Mock<ICache>();
				FileSystem = new Mock<IFileSystem>();
				FileCacheHash = new Mock<IFileCacheHash>();
				ReactIdGenerator = new Mock<IReactIdGenerator>();

				EngineFactory.Setup(x => x.GetEngine()).Returns(Engine.Object);
				EngineFactory.Setup(x => x.GetEngineForCurrentThread()).Returns(Engine.Object);
				Config.Setup(x => x.LoadBabel).Returns(true);
			}

			public ReactEnvironment CreateReactEnvironment()
			{
				return new ReactEnvironment(
					EngineFactory.Object,
					Config.Object,
					Cache.Object,
					FileSystem.Object,
					FileCacheHash.Object,
					ReactIdGenerator.Object
				);
			}

			public Mock<ReactEnvironment> CreateMockedReactEnvironment()
			{
				return new Mock<ReactEnvironment>(
					EngineFactory.Object,
					Config.Object,
					Cache.Object,
					FileSystem.Object,
					FileCacheHash.Object
				);
			}
		}
	}
}
