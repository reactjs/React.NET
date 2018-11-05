/*
 * Copyright (c) Facebook, Inc. and its affiliates.
 *
 * This source code is licensed under the MIT license found in the
 * LICENSE file in the root directory of this source tree.
 */

using React.TinyIoC;

namespace React.AspNet
{
	/// <summary>
	/// Handles registration of ReactJS.NET components that are only applicable
	/// in the context of an ASP.NET web application.
	/// </summary>
	public class AssemblyRegistration : IAssemblyRegistration
	{
		/// <summary>
		/// Registers components in the React IoC container
		/// </summary>
		/// <param name="container">Container to register components in</param>
		public void Register(TinyIoCContainer container)
		{
			container.Register<IFileSystem, AspNetFileSystem>().AsSingleton();
			container.Register<ICache, MemoryFileCacheCore>().AsSingleton();
		}
	}
}
