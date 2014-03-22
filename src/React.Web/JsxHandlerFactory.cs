/*
 *  Copyright (c) 2014, Facebook, Inc.
 *  All rights reserved.
 *
 *  This source code is licensed under the BSD-style license found in the
 *  LICENSE file in the root directory of this source tree. An additional grant 
 *  of patent rights can be found in the PATENTS file in the same directory.
 */

using System.Web;

namespace React.Web
{
	/// <summary>
	/// Handles creation and execution of <see cref="IJsxHandler"/> instances.
	/// </summary>
	public class JsxHandlerFactory : IHttpHandler
	{
		/// <summary>
		/// Processes this request
		/// </summary>
		/// <param name="context">The request context</param>
		public void ProcessRequest(HttpContext context)
		{
			var handler = React.AssemblyRegistration.Container.Resolve<IJsxHandler>();
			handler.Execute();
		}

		/// <summary>
		/// Gets a value indicating whether another request can use the <see cref="T:System.Web.IHttpHandler" /> instance.
		/// </summary>
		/// <returns>true if the <see cref="T:System.Web.IHttpHandler" /> instance is reusable; otherwise, false.</returns>
		public bool IsReusable { get { return false; } }
	}
}
