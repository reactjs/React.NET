/*
 *  Copyright (c) 2014-Present, Facebook, Inc.
 *  All rights reserved.
 *
 *  This source code is licensed under the BSD-style license found in the
 *  LICENSE file in the root directory of this source tree. An additional grant
 *  of patent rights can be found in the PATENTS file in the same directory.
 */

using Moq;
using Xunit;
using React.Exceptions;
using React.Router;
using React.Tests.Core;

namespace React.Tests.Router
{
	public class ReactEnvironmentExtensionsTest
	{
		/// <summary>
		/// Creates a mock <see cref="IReactEnvironment"/> and registers it with the IoC container
		/// This is only required because <see cref="HtmlHelperExtensions"/> can not be
		/// injected :(
		/// </summary>
		private ReactEnvironment ConfigureMockReactEnvironment()
		{
			var mocks = new ReactEnvironmentTest.Mocks();

			var environment = mocks.CreateReactEnvironment();
			AssemblyRegistration.Container.Register<IReactEnvironment>(environment);
			return environment;
		}

  //      [Fact]
		//public void EnvironmentShouldGetCalledClientOnly()
		//{
  //          var environment = ConfigureMockReactEnvironment();
		//	var component = ReactEnvironmentExtensions.CreateRouterComponent(
		//		environment.Object,
		//		"ComponentName",
		//		new { },
		//		"/",
		//		null,
		//		true
		//	);

		//	environment.Verify(x => x.CreateComponent("ComponentName", new { }, null, true));
		//}
	}
}
