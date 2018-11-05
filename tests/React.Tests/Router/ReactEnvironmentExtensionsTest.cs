/*
 * Copyright (c) Facebook, Inc. and its affiliates.
 *
 * This source code is licensed under the MIT license found in the
 * LICENSE file in the root directory of this source tree.
 */
#if NET452 || NETCOREAPP2_0

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
#endif
