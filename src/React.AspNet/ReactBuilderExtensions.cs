/*
 *  Copyright (c) 2015, Facebook, Inc.
 *  All rights reserved.
 *
 *  This source code is licensed under the BSD-style license found in the
 *  LICENSE file in the root directory of this source tree. An additional grant 
 *  of patent rights can be found in the PATENTS file in the same directory.
 */

using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using React.Exceptions;
using React.TinyIoC;
using Microsoft.Extensions.DependencyInjection;
#if !NET451
using Microsoft.Extensions.Caching.Memory;
#endif

namespace React.AspNet
{
	/// <summary>
	/// Handles registering ReactJS.NET middleware in an ASP.NET <see cref="IApplicationBuilder"/>.
	/// </summary>
    public static class ReactBuilderExtensions
    {
		/// <summary>
		/// Initialises ReactJS.NET for this application
		/// </summary>
		/// <param name="app">ASP.NET application builder</param>
		/// <param name="configure">ReactJS.NET configuration</param>
		/// <param name="fileOptions">Options to use for serving files</param>
		/// <returns>The application builder (for chaining)</returns>
		public static IApplicationBuilder UseReact(
			this IApplicationBuilder app,
			Action<IReactSiteConfiguration> configure,
			BabelFileOptions fileOptions = null
		)
		{
			EnsureServicesRegistered(app);

			RegisterAspNetServices(React.AssemblyRegistration.Container, app.ApplicationServices);

			Initializer.Initialize(registerOptions => AsPerRequestSingleton(
				app.ApplicationServices.GetService<IHttpContextAccessor>(), 
				registerOptions
			));
			configure(ReactSiteConfiguration.Configuration);

			// Allow serving of .jsx files
			app.UseMiddleware<BabelFileMiddleware>(fileOptions ?? new BabelFileOptions());

			return app;
		}

		/// <summary>
		/// Registers a class such that every ASP.NET web request has a single instance of it.
		/// </summary>
		/// <param name="httpContextAccessor">ASP.NET HTTP context accessor</param>
		/// <param name="registerOptions">Registration options</param>
		/// <returns>Registration options (for chaining)</returns>
		private static TinyIoCContainer.RegisterOptions AsPerRequestSingleton(
			IHttpContextAccessor httpContextAccessor,
			TinyIoCContainer.RegisterOptions registerOptions
		)
		{
			return TinyIoCContainer.RegisterOptions.ToCustomLifetimeManager(
				registerOptions,
				new HttpContextLifetimeProvider(httpContextAccessor),
				"per request singleton"
			);
		}

		/// <summary>
		/// Ensures React services have been registered in the ASP.NET dependency injection container.
		/// </summary>
		/// <param name="app">ASP.NET application builder</param>
		private static void EnsureServicesRegistered(IApplicationBuilder app)
		{
			var registrations = app.ApplicationServices.GetService<HttpContextLifetimeProvider.PerRequestRegistrations>();
			if (registrations == null)
			{
				throw new ReactNotInitialisedException("Please call app.AddReact() before app.UseReact().");
			}
		}

		/// <summary>
		/// Registers required ASP.NET services in ReactJS.NET's TinyIoC container. This is used
		/// for ASP.NET services that are required by ReactJS.NET.
		/// </summary>
		/// <param name="container">ReactJS.NET dependency injection container</param>
		/// <param name="services">ASP.NET dependency injection container</param>
		private static void RegisterAspNetServices(TinyIoCContainer container, IServiceProvider services)
		{
			container.Register(services.GetRequiredService<IHostingEnvironment>());
#if !NET451
			container.Register(services.GetRequiredService<IMemoryCache>());
#endif
		}
	}
}