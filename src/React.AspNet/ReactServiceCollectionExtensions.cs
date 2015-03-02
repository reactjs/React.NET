/*
 *  Copyright (c) 2015, Facebook, Inc.
 *  All rights reserved.
 *
 *  This source code is licensed under the BSD-style license found in the
 *  LICENSE file in the root directory of this source tree. An additional grant 
 *  of patent rights can be found in the PATENTS file in the same directory.
 */

using Microsoft.Framework.DependencyInjection;

namespace React.AspNet
{
	/// <summary>
	/// Handles registering ReactJS.NET services in the ASP.NET <see cref="IServiceCollection"/>.
	/// </summary>
	public static class ReactServiceCollectionExtensions
	{
		/// <summary>
		/// Registers all services required for ReactJS.NET
		/// </summary>
		/// <param name="services">ASP.NET services</param>
		/// <returns>The service collection (for chaining)</returns>
		public static IServiceCollection AddReact(this IServiceCollection services)
		{
			services.AddScoped<HttpContextLifetimeProvider.PerRequestRegistrations>();
			return services;
		}
	}
}