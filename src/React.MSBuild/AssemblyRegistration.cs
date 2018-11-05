/*
 * Copyright (c) Facebook, Inc. and its affiliates.
 *
 * This source code is licensed under the MIT license found in the
 * LICENSE file in the root directory of this source tree.
 */

using System.Diagnostics;
using React.TinyIoC;

namespace React.MSBuild
{
	/// <summary>
	/// Handles registration of ReactJS.NET components that are only applicable
	/// when used with MSBuild
	/// </summary>
	public class AssemblyRegistration : IAssemblyRegistration
	{
		/// <summary>
		/// Registers components in the React IoC container
		/// </summary>
		/// <param name="container">Container to register components in</param>
		public void Register(TinyIoCContainer container)
		{
			if (!MSBuildHost.IsInMSBuild())
			{
				Trace.WriteLine(
					"Warning: React.MSBuild AssemblyRegistration called, but not currently in MSBuild!"
				);
				return;
			}

			container.Register<ICache, NullCache>();
			container.Register<IFileSystem, SimpleFileSystem>();
		}
	}
}
