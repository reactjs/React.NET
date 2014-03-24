/*
 *  Copyright (c) 2014, Facebook, Inc.
 *  All rights reserved.
 *
 *  This source code is licensed under the BSD-style license found in the
 *  LICENSE file in the root directory of this source tree. An additional grant 
 *  of patent rights can be found in the PATENTS file in the same directory.
 */

using System.Threading;
using JavaScriptEngineSwitcher.Core;
using Xunit;

namespace React.Tests.Core
{
	public class JavaScriptEngineFactoryTest
	{
		[Fact]
		public void ShouldCallOnNewEngineWhenCreatingNew()
		{
			var factory = new JavaScriptEngineFactory();
			var called = false;
			factory.GetEngineForCurrentThread(engine =>
			{
				Assert.NotNull(engine);
				called = true;
			});
			factory.DisposeEngineForCurrentThread();

			Assert.True(called);
		}

		[Fact]
		public void ShouldNotCallOnNewEngineWhenUsingExisting()
		{
			var factory = new JavaScriptEngineFactory();
			var called = false;
			factory.GetEngineForCurrentThread();
			factory.GetEngineForCurrentThread(engine => { called = true; });
			factory.DisposeEngineForCurrentThread();

			Assert.False(called);
		}

		[Fact]
		public void ShouldReturnSameEngine()
		{
			var factory = new JavaScriptEngineFactory();
			var engine1 = factory.GetEngineForCurrentThread();
			var engine2 = factory.GetEngineForCurrentThread();
			
			Assert.Equal(engine1, engine2);
			factory.DisposeEngineForCurrentThread();
		}

		[Fact]
		public void ShouldReturnNewEngineAfterDisposing()
		{
			var factory = new JavaScriptEngineFactory();
			var engine1 = factory.GetEngineForCurrentThread();
			factory.DisposeEngineForCurrentThread();
			var engine2 = factory.GetEngineForCurrentThread();
			factory.DisposeEngineForCurrentThread();

			Assert.NotEqual(engine1, engine2);
		}

		[Fact]
		public void ShouldCreateNewEngineForNewThread()
		{
			var factory = new JavaScriptEngineFactory();
			var engine1 = factory.GetEngineForCurrentThread();

			IJsEngine engine2 = null;
			var thread = new Thread(() =>
			{
				engine2 = factory.GetEngineForCurrentThread();
				// Need to ensure engine is disposed in same thread as it was created in
				factory.DisposeEngineForCurrentThread();
			});

			var engine3 = factory.GetEngineForCurrentThread();

			// Different threads should have different engines
			Assert.NotEqual(engine1, engine2);
			// Same thread should share same engine
			Assert.Equal(engine1, engine3);
			factory.DisposeEngineForCurrentThread();
		}
	}
}
