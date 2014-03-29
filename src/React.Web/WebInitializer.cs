/*
 *  Copyright (c) 2014, Facebook, Inc.
 *  All rights reserved.
 *
 *  This source code is licensed under the BSD-style license found in the
 *  LICENSE file in the root directory of this source tree. An additional grant 
 *  of patent rights can be found in the PATENTS file in the same directory.
 */

using System.Web;
using Microsoft.Web.Infrastructure.DynamicModuleHelper;
using React.TinyIoC;
using React.Web;

[assembly: WebActivatorEx.PreApplicationStartMethod(typeof(WebInitializer), "Initialize")]

namespace React.Web
{
	/// <summary>
	/// Handles initialisation of ReactJS.NET. This is only called once, at application start.
	/// </summary>
	internal static class WebInitializer
	{
		/// <summary>
		/// Intialise ReactJS.NET
		/// </summary>
		public static void Initialize()
		{
			Initializer.Initialize(isInAspNet: true);
			DynamicModuleUtility.RegisterModule(typeof(IocPerRequestDisposal));
		}

		/// <summary>
		/// Handles disposing per-request IoC instances at the end of the request
		/// </summary>
		private class IocPerRequestDisposal : IHttpModule
		{
			public void Init(HttpApplication context)
			{
				context.EndRequest += (sender, args) => HttpContextLifetimeProvider.DisposeAll();
			}
			public void Dispose() { }
		}
	}
}
