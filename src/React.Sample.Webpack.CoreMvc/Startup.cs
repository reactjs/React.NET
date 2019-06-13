using System;
using JavaScriptEngineSwitcher.ChakraCore;
using JavaScriptEngineSwitcher.Extensions.MsDependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using React.AspNet;

namespace React.Sample.Webpack.CoreMvc
{
	public class Startup
	{
		public Startup(IConfiguration configuration)
		{
			Configuration = configuration;
		}

		public IConfiguration Configuration { get; }

		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices(IServiceCollection services)
		{
			services.AddMvc();

			services.AddJsEngineSwitcher(options => options.DefaultEngineName = ChakraCoreJsEngine.EngineName)
				.AddChakraCore();

			services.AddReact();
			services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

			// Build the intermediate service provider then return it
			services.BuildServiceProvider();
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app)
		{
			// Initialise ReactJS.NET. Must be before static files.
			app.UseReact(config =>
			{
				config
					.SetReuseJavaScriptEngines(true)
					.SetLoadBabel(false)
					.SetLoadReact(false)
					.AddScriptWithoutTransform("~/dist/runtime.js")
					.AddScriptWithoutTransform("~/dist/vendor.js")
					.AddScriptWithoutTransform("~/dist/components.js");
			});

			app.UseStaticFiles();

#if NETCOREAPP3_0
			app.UseRouting();

			app.UseEndpoints(endpoints =>
			{
				endpoints.MapControllerRoute("default", "{path?}", new { controller = "Home", action = "Index" });
				endpoints.MapControllerRoute("comments-root", "comments", new { controller = "Home", action = "Index" });
				endpoints.MapControllerRoute("comments", "comments/page-{page}", new { controller = "Home", action = "Comments" });
			});
#else
			app.UseMvc(routes =>
			{
				routes.MapRoute("default", "{path?}", new { controller = "Home", action = "Index" });
				routes.MapRoute("comments-root", "comments", new { controller = "Home", action = "Index" });
				routes.MapRoute("comments", "comments/page-{page}", new { controller = "Home", action = "Comments" });
			});
#endif
		}
	}
}
