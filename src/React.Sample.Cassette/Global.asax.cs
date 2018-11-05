/*
 * Copyright (c) Facebook, Inc. and its affiliates.
 *
 * This source code is licensed under the MIT license found in the
 * LICENSE file in the root directory of this source tree.
 */

using System.Web.Hosting;
using System.Web.Mvc;
using System.Web.Routing;
using React.Sample.Mvc4;

namespace React.Sample.Cassette
{
	// Note: For instructions on enabling IIS6 or IIS7 classic mode, 
	// visit http://go.microsoft.com/?LinkId=9394801
	public class MvcApplication : System.Web.HttpApplication
	{
		protected void Application_Start()
		{
			Initializer.Initialize(registration => registration.AsSingleton());
			var container = React.AssemblyRegistration.Container;

			container.Register<ICache, NullCache>();
			container.Register<IFileSystem, AspNetFileSystem>();

			AreaRegistration.RegisterAllAreas();

			FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
			RouteConfig.RegisterRoutes(RouteTable.Routes);
		}

		private class AspNetFileSystem : FileSystemBase
		{
			/// <summary>
			/// Converts a path from an application relative path (~/...) to a full filesystem path
			/// </summary>
			/// <param name="relativePath">App-relative path of the file</param>
			/// <returns>Full path of the file</returns>
			public override string MapPath(string relativePath)
			{
				return HostingEnvironment.MapPath(relativePath);
			}
		}
	}
}
