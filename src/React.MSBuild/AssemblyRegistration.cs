/*
 *  Copyright (c) 2014, Facebook, Inc.
 *  All rights reserved.
 *
 *  This source code is licensed under the BSD-style license found in the
 *  LICENSE file in the root directory of this source tree. An additional grant 
 *  of patent rights can be found in the PATENTS file in the same directory.
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
