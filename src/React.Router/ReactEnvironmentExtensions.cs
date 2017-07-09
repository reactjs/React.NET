﻿/*
 *  Copyright (c) 2014-Present, Facebook, Inc.
 *  All rights reserved.
 *
 *  This source code is licensed under the BSD-style license found in the
 *  LICENSE file in the root directory of this source tree. An additional grant
 *  of patent rights can be found in the PATENTS file in the same directory.
 */

namespace React.Router
{
	/// <summary>
	/// <see cref="ReactEnvironment"/> extension for rendering a React Router Component with context
	/// </summary>
	public static class ReactEnvironmentExtensions
	{
		/// <summary>
		/// Create a React Router Component with context and add it to the list of components to render client side,
		/// if applicable.
		/// </summary>
		/// <typeparam name="T">Type of the props</typeparam>
		/// <param name="env">React Environment</param>
		/// <param name="componentName">Name of the component</param>
		/// <param name="props">Props to use</param>
		/// <param name="path">F.x. from Request.Path. Used by React Static Router to determine context and routing.</param>
		/// <param name="containerId">ID to use for the container HTML tag. Defaults to an auto-generated ID</param>
		/// <param name="clientOnly">True if server-side rendering will be bypassed. Defaults to false.</param>
		/// <returns></returns>
		public static ReactRouterComponent CreateRouterComponent<T>(
			this IReactEnvironment env,
			string componentName,
			T props,
			string path,
			string containerId = null,
			bool clientOnly = false
		)
		{
			var config = AssemblyRegistration.Container.Resolve<IReactSiteConfiguration>();

			var component = new ReactRouterComponent(
				env, 
				config, 
				componentName, 
				containerId, 
				path
			)
			{
				Props = props,
			};

			return env.CreateComponent(component, clientOnly) as ReactRouterComponent;
		}
	}
}
