using System;
using Microsoft.AspNetCore.Builder;
using React.AspNet;
using React.TinyIoC;

namespace React.NodeServices
{
	public static class ReactNodeEnvironmentExtensions
	{
		/// <summary>
		/// 
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="app"></param>
		/// <param name="configure"></param>
		/// <param name="fileOptions"></param>
		/// <returns></returns>
		public static IApplicationBuilder UseReactWithEnvironment<T>(
			this IApplicationBuilder app,
			Action<IReactSiteConfiguration> configure,
			BabelFileOptions fileOptions = null
		) where T : class, IReactEnvironment
		{
			var appInstance = app.UseReact(configure, fileOptions);
			TinyIoCContainer.Current.Unregister<IReactEnvironment>();
			TinyIoCContainer.Current.Register<IReactEnvironment, T>();

			return appInstance;
		}
	}
}
