/*
 * Copyright (c) Facebook, Inc. and its affiliates.
 *
 * This source code is licensed under the MIT license found in the
 * LICENSE file in the root directory of this source tree.
 */

using System.Diagnostics;
using System.Web;
using System.Web.Caching;
using System.Web.Hosting;
using React.TinyIoC;

namespace React.Web
{
	/// <summary>
	/// Handles registration of ReactJS.NET components that are only applicable
	/// in the context of an ASP.NET web application.
	/// </summary>
	public class AssemblyRegistration : IAssemblyRegistration
	{
		/// <summary>
		/// Registers components in the React IoC container
		/// </summary>
		/// <param name="container">Container to register components in</param>
		public void Register(TinyIoCContainer container)
		{
			if (!IsInAspNet())
			{
				Trace.WriteLine(
					"Warning: The current application references React.Web but is not an " +
					"ASP.NET Web Application. Not running webapp IoC initialisation!"
				);
				return;
			}

			// Unique per request
			container.Register<IFileSystem, AspNetFileSystem>().AsPerRequestSingleton();
			container.Register<IBabelHandler, BabelHandler>().AsPerRequestSingleton();

			// Mono for Mac OS does not properly handle caching
			// TODO: Remove this once https://bugzilla.xamarin.com/show_bug.cgi?id=19071 is fixed
			if (SystemEnvironmentUtils.IsRunningOnMac())
			{
				container.Register<ICache, NullCache>().AsSingleton();
			}
			else
			{
				container.Register<ICache, AspNetCache>().AsPerRequestSingleton();
			}

			// Wrappers for built-in objects
			container.Register<HttpContextBase>((c, o) => new HttpContextWrapper(HttpContext.Current));
			container.Register<HttpServerUtilityBase>((c, o) => c.Resolve<HttpContextBase>().Server);
			container.Register<HttpRequestBase>((c, o) => c.Resolve<HttpContextBase>().Request);
			container.Register<HttpResponseBase>((c, o) => c.Resolve<HttpContextBase>().Response);
			container.Register<Cache>((c, o) => HttpRuntime.Cache);
		}

		/// <summary>
		/// Determines if the current application is running in the context of an ASP.NET
		/// Web Application
		/// </summary>
		/// <returns><c>true</c> if in an ASP.NET web app; <c>false</c> otherwise</returns>
		public static bool IsInAspNet()
		{
			return HostingEnvironment.IsHosted;
		}
	}
}
