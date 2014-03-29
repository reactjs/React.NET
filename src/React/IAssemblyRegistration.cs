/*
 *  Copyright (c) 2014, Facebook, Inc.
 *  All rights reserved.
 *
 *  This source code is licensed under the BSD-style license found in the
 *  LICENSE file in the root directory of this source tree. An additional grant 
 *  of patent rights can be found in the PATENTS file in the same directory.
 */

using React.TinyIoC;

namespace React
{
	/// <summary>
	/// IoC component registration. Used to register components in the ReactJS.NET IoC container. 
	/// Every ReactJS.NET assembly should have an instance of IComponentRegistration.
	/// </summary>
	public interface IAssemblyRegistration
	{
		/// <summary>
		/// Registers components in the ReactJS.NET IoC container
		/// </summary>
		/// <param name="container">Container to register components in</param>
		void Register(TinyIoCContainer container);
	}
}
