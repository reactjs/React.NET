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
		[Fact]
		public void EnvironmentShouldGetCalledClientOnly()
		{
			var environment = new Mock<IReactEnvironment>();
			AssemblyRegistration.Container.Register(environment.Object);
			var config = new Mock<IReactSiteConfiguration>();
			AssemblyRegistration.Container.Register(config.Object);
			var reactIdGenerator = new Mock<IReactIdGenerator>();
			AssemblyRegistration.Container.Register(reactIdGenerator.Object);

			var component = ReactEnvironmentExtensions.CreateRouterComponent(
				environment.Object,
				"ComponentName",
				new { },
				"/",
				null,
				true
			);

			environment.Verify(x => x.CreateComponent(It.IsAny<IReactComponent>(), true));
		}
	}
}
