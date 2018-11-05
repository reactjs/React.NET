/*
 * Copyright (c) Facebook, Inc. and its affiliates.
 *
 * This source code is licensed under the MIT license found in the
 * LICENSE file in the root directory of this source tree.
 */

using React.TinyIoC;

namespace React.Owin
{
	/// <summary>
	/// Handles registration of ReactJS.NET components that are only applicable
	/// when used with Owin.
	/// </summary>
	public class AssemblyRegistration : IAssemblyRegistration
	{
		/// <summary>
		/// Registers components in the React IoC container
		/// </summary>
		/// <param name="container">Container to register components in</param>
		public void Register(TinyIoCContainer container)
		{
			container.Register<IFileSystem, EntryAssemblyFileSystem>();
			container.Register<ICache, MemoryFileCache>();
		}
	}
}
