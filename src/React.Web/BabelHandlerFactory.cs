﻿/*
 * Copyright (c) Facebook, Inc. and its affiliates.
 *
 * This source code is licensed under the MIT license found in the
 * LICENSE file in the root directory of this source tree.
 */

using System.Web;

namespace React.Web
{
	/// <summary>
	/// Handles creation and execution of <see cref="IBabelHandler"/> instances.
	/// </summary>
	public class BabelHandlerFactory : IHttpHandler
	{
		/// <summary>
		/// Processes this request
		/// </summary>
		/// <param name="context">The request context</param>
		public void ProcessRequest(HttpContext context)
		{
			var handler = React.AssemblyRegistration.Container.Resolve<IBabelHandler>();
			handler.Execute();
		}

		/// <summary>
		/// Gets a value indicating whether another request can use the <see cref="T:System.Web.IHttpHandler" /> instance.
		/// </summary>
		/// <returns>true if the <see cref="T:System.Web.IHttpHandler" /> instance is reusable; otherwise, false.</returns>
		public bool IsReusable { get { return false; } }
	}
}
