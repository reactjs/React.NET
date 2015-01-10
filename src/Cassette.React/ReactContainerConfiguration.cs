/*
 *  Copyright (c) 2014-2015, Facebook, Inc.
 *  All rights reserved.
 *
 *  This source code is licensed under the BSD-style license found in the
 *  LICENSE file in the root directory of this source tree. An additional grant 
 *  of patent rights can be found in the PATENTS file in the same directory.
 */

using CassetteTinyIoCContainer = Cassette.TinyIoC.TinyIoCContainer;
using ReactTinyIoCContainer = React.TinyIoC.TinyIoCContainer;
using React;

namespace Cassette.React
{
	/// <summary>
	/// Cassette IoC configuration for ReactJS.NET
	/// </summary>
	public class ReactContainerConfiguration : IConfiguration<CassetteTinyIoCContainer>
	{
		/// <summary>
		/// Configures the specified Cassette IoC container.
		/// </summary>
		/// <param name="container">The IoC container.</param>
		public void Configure(CassetteTinyIoCContainer container)
		{
			// Register ReactJS.NET's IoC container inside Cassette's.
			container.Register<ReactTinyIoCContainer>(global::React.AssemblyRegistration.Container);
			RegisterPassthru<IReactEnvironment>(container);
		}

		/// <summary>
		/// Registers a component in Cassette's IoC container that just delegates resolution to 
		/// ReactJS.NET's IoC container.
		/// </summary>
		/// <typeparam name="T">Type to register</typeparam>
		/// <param name="container">Cassette's IoC container</param>
		private void RegisterPassthru<T>(CassetteTinyIoCContainer container) where T : class
		{
			container.Register<T>((c, overloads) => global::React.AssemblyRegistration.Container.Resolve<T>());
		}
	}
}
