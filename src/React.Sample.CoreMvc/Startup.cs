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
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using React.AspNet;

namespace React.Sample.CoreMvc
{
	public class Startup
	{
		public Startup(IHostingEnvironment env)
		{
			// Setup configuration sources.
			var builder = new ConfigurationBuilder().AddEnvironmentVariables();

			Configuration = builder.Build();
		}

		public IConfiguration Configuration { get; set; }

		// This method gets called by the runtime.
		public IServiceProvider ConfigureServices(IServiceCollection services)
		{
			// Add MVC services to the services container.
			services.AddMvc();

			services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

			// Add ReactJS.NET services.
			services.AddReact();

			// Build the intermediate service provider then return it
			return services.BuildServiceProvider();
		}

		// Configure is called after ConfigureServices is called.
		public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerfactory)
		{ 
			// Add the console logger.
			loggerfactory.AddConsole();

			// Add the following to the request pipeline only in development environment.
			if (env.IsDevelopment())
			{
				//app.UseBrowserLink();
				app.UseDeveloperExceptionPage();
			}
			else
			{
				// Add Error handling middleware which catches all application specific errors and
				// send the request to the following path or controller action.
				app.UseExceptionHandler("/Home/Error");
			}
			
			// Initialise ReactJS.NET. Must be before static files.
			app.UseReact(config =>
			{
				config
					.SetReuseJavaScriptEngines(true)
					.AddScript("~/js/Sample.jsx")
					.SetUseDebugReact(true);
			});

			// Add static files to the request pipeline.
			app.UseStaticFiles();

			// Add MVC to the request pipeline.
			app.UseMvc(routes =>
			{
				routes.MapRoute(
					name: "Comments",
					template: "comments/page-{page}",
					defaults: new { controller = "Home", action = "Comments", page = 1 }
				);

				routes.MapRoute(
					name: "default",
					template: "{controller}/{action}/{id?}",
					defaults: new { controller = "Home", action = "Index" }
				);
			});
		}
	}
}
