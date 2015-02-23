/*
 *  Copyright (c) 2015, Facebook, Inc.
 *  All rights reserved.
 *
 *  This source code is licensed under the BSD-style license found in the
 *  LICENSE file in the root directory of this source tree. An additional grant 
 *  of patent rights can be found in the PATENTS file in the same directory.
 */

using React.TinyIoC;

namespace React.AspNet5
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
			container.Register<IFileSystem, AspNet5FileSystem>().AsSingleton();
			container.Register<ICache, MemoryFileCache>().AsSingleton();
		}
	}
}
