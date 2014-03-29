/*
 *  Copyright (c) 2014, Facebook, Inc.
 *  All rights reserved.
 *
 *  This source code is licensed under the BSD-style license found in the
 *  LICENSE file in the root directory of this source tree. An additional grant 
 *  of patent rights can be found in the PATENTS file in the same directory.
 */

using System.Web;
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
			// Unique per request
			container.Register<IFileSystem, AspNetFileSystem>().AsPerRequestSingleton();
			container.Register<ICache, AspNetCache>().AsPerRequestSingleton();
			container.Register<IJsxHandler, JsxHandler>().AsPerRequestSingleton();

			// Wrappers for built-in objects
			container.Register<HttpContextBase>((c, o) => new HttpContextWrapper(HttpContext.Current));
			container.Register<HttpServerUtilityBase>((c, o) => c.Resolve<HttpContextBase>().Server);
			container.Register<HttpRequestBase>((c, o) => c.Resolve<HttpContextBase>().Request);
			container.Register<HttpResponseBase>((c, o) => c.Resolve<HttpContextBase>().Response);
		}
	}
}
