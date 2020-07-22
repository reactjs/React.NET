/*
 * Copyright (c) Facebook, Inc. and its affiliates.
 *
 * This source code is licensed under the MIT license found in the
 * LICENSE file in the root directory of this source tree.
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
			var component = new ReactRouterComponent(
				env,
				AssemblyRegistration.Container.Resolve<IReactSiteConfiguration>(), 
				AssemblyRegistration.Container.Resolve<IReactIdGenerator>(),
				componentName, 
				containerId, 
				path
			)
			{
				Props = props,
				ClientOnly = clientOnly,
			};

			return env.CreateComponent(component, clientOnly) as ReactRouterComponent;
		}
	}
}
