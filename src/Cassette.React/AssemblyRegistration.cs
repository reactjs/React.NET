/*
 * Copyright (c) Facebook, Inc. and its affiliates.
 *
 * This source code is licensed under the MIT license found in the
 * LICENSE file in the root directory of this source tree.
 */

using React;
using React.TinyIoC;

namespace Cassette.React
{
	/// <summary>
	/// Handles registration of ReactJS.NET components that are only applicable
	/// to Cassette when used in MSBuild.
	/// </summary>
	public class AssemblyRegistration : IAssemblyRegistration
	{
		/// <summary>
		/// Registers components in the React IoC container
		/// </summary>
		/// <param name="container">Container to register components in</param>
		public void Register(TinyIoCContainer container)
		{
			if (MSBuildUtils.IsInMSBuild())
			{
				RegisterForMSBuild(container);
			}
		}

		/// <summary>
		/// Registers components specific to the MSBuild environment in the React IoC container.
		/// </summary>
		/// <param name="container">Container to register components in</param>
		private void RegisterForMSBuild(TinyIoCContainer container)
		{
			container.Register<ICache, NullCache>();
			container.Register<IFileSystem, SimpleFileSystem>();
		}
	}
}
