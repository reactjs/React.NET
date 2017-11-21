using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using React.AspNet;
using System.Runtime.InteropServices; // Required to work on .NET Core on Windows
using VroomJs; // Required to work on .NET Core on Windows

namespace ReactDemo
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
			services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
			services.AddReact();
            // Add framework services.
            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            // For V8 to work on .NET Core on Windows we need to ensure the assemblies are loaded.
            // This is not required for the full .NET Framework - its presence is not an issue for running on the full framework though
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                AssemblyLoader.EnsureLoaded();

            // Initialise ReactJS.NET. Must be before static files.
            app.UseReact(config =>
			{
				// If you want to use server-side rendering of React components,
				// add all the necessary JavaScript files here. This includes
				// your components as well as all of their dependencies.
				// See http://reactjs.net/ for more information. Example:
				config
				  .AddScript("~/js/remarkable.min.js")
				  .AddScript("~/js/tutorial.jsx")
				  .SetJsonSerializerSettings(new JsonSerializerSettings
					{
						StringEscapeHandling = StringEscapeHandling.EscapeHtml,
						ContractResolver = new CamelCasePropertyNamesContractResolver()
					});

				// If you use an external build too (for example, Babel, Webpack,
				// Browserify or Gulp), you can improve performance by disabling
				// ReactJS.NET's version of Babel and loading the pre-transpiled
				// scripts. Example:
				//config
				//  .SetLoadBabel(false)
				//  .AddScriptWithoutTransform("~/Scripts/bundle.server.js");
			});

			app.UseStaticFiles();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
