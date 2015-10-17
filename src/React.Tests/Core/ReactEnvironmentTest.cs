/*
 *  Copyright (c) 2014-2015, Facebook, Inc.
 *  All rights reserved.
 *
 *  This source code is licensed under the BSD-style license found in the
 *  LICENSE file in the root directory of this source tree. An additional grant 
 *  of patent rights can be found in the PATENTS file in the same directory.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using JavaScriptEngineSwitcher.Core;
using Moq;
using NUnit.Framework;
using React.Exceptions;

namespace React.Tests.Core
{
	[TestFixture]
	public class ReactEnvironmentTest
	{
		[Test]
		public void ExecuteWithBabelWithNoNewThread()
		{
			var mocks = new Mocks();
			var environment = mocks.CreateReactEnvironment();

			environment.ExecuteWithBabel<int>("foo");
			mocks.Engine.Verify(x => x.CallFunction<int>("foo"), Times.Exactly(1));
		}

		[Test]
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

		[Test]
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

		[Test]
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

		[Test]
		public void GeneratesContainerIdIfNotProvided()
		{
			var mocks = new Mocks();
			var environment = mocks.CreateReactEnvironment();
			mocks.Config.Setup(x => x.Scripts).Returns(new List<string>());

			var component1 = environment.CreateComponent("ComponentName", new { });
			var component2 = environment.CreateComponent("ComponentName", new { });
			Assert.AreEqual("react1", component1.ContainerId);
			Assert.AreEqual("react2", component2.ContainerId);
		}

		[Test]
		public void UsesProvidedContainerId()
		{
			var mocks = new Mocks();
			var environment = mocks.CreateReactEnvironment();
			mocks.Config.Setup(x => x.Scripts).Returns(new List<string>());

			var component1 = environment.CreateComponent("ComponentName", new { }, "foo");
			var component2 = environment.CreateComponent("ComponentName", new { });
			Assert.AreEqual("foo", component1.ContainerId);
			Assert.AreEqual("react1", component2.ContainerId);
		}

		private class Mocks
		{
			public Mock<IJsEngine> Engine { get; private set; }
			public Mock<IJavaScriptEngineFactory> EngineFactory { get; private set; }
			public Mock<IReactSiteConfiguration> Config { get; private set; }
			public Mock<ICache> Cache { get; private set; }
			public Mock<IFileSystem> FileSystem { get; private set; }
			public Mock<IFileCacheHash> FileCacheHash { get; private set; }
			public Mocks()
			{
				Engine = new Mock<IJsEngine>();
				EngineFactory = new Mock<IJavaScriptEngineFactory>();
				Config = new Mock<IReactSiteConfiguration>();
				Cache = new Mock<ICache>();
				FileSystem = new Mock<IFileSystem>();
				FileCacheHash = new Mock<IFileCacheHash>();

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
					FileCacheHash.Object
				);
			}
		}
	}
}