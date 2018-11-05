/*
 * Copyright (c) Facebook, Inc. and its affiliates.
 *
 * This source code is licensed under the MIT license found in the
 * LICENSE file in the root directory of this source tree.
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
