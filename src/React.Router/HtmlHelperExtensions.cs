/*
 *  Copyright (c) 2014-Present, Facebook, Inc.
 *  All rights reserved.
 *
 *  This source code is licensed under the BSD-style license found in the
 *  LICENSE file in the root directory of this source tree. An additional grant
 *  of patent rights can be found in the PATENTS file in the same directory.
 */

using System;

#if LEGACYASPNET
using System.Web;
using HttpResponse = System.Web.HttpResponseBase;
using IHtmlHelper = System.Web.Mvc.HtmlHelper;
#else
using Microsoft.AspNetCore.Mvc.Rendering;
using IHtmlString = Microsoft.AspNetCore.Html.IHtmlContent;
using HttpResponse = Microsoft.AspNetCore.Http.HttpResponse;
using Microsoft.AspNetCore.Html;
#endif

namespace React.Router
{
	/// <summary>
	/// Render a React StaticRouter Component with context.
	/// </summary>
	public static class HtmlHelperExtensions
	{
		/// <summary>
		/// Gets the React environment
		/// </summary>
		private static IReactEnvironment Environment
		{
			get
			{
				return ReactEnvironment.GetCurrentOrThrow;
			}
		}

		/// <summary>
		/// Render a React StaticRouter Component with context object.
		/// Can optionally be provided with a custom context handler to handle the various status codes.
		/// </summary>
		/// <param name="htmlHelper">MVC Razor <see cref="IHtmlHelper"/></param>
		/// <param name="componentName">Name of React Static Router component. Expose component globally to ReactJS.NET</param>
		/// <param name="props">Props to initialise the component with</param>
		/// <param name="path">F.x. from Request.Path. Used by React Static Router to determine context and routing.</param>
		/// <param name="contextHandler">Optional custom context handler, can be used instead of providing a Response object</param>
		/// <param name="htmlTag">HTML tag to wrap the component in. Defaults to &lt;div&gt;</param>
		/// <param name="containerId">ID to use for the container HTML tag. Defaults to an auto-generated ID</param>
		/// <param name="clientOnly">Skip rendering server-side and only output client-side initialisation code. Defaults to <c>false</c></param>
		/// <param name="serverOnly">Skip rendering React specific data-attributes during server side rendering. Defaults to <c>false</c></param>
		/// <param name="containerClass">HTML class(es) to set on the container tag</param>
		/// <returns><see cref="IHtmlString"/> containing the rendered markup for provided React Router component</returns>
		public static IHtmlString ReactRouterWithContext<T>(
			this IHtmlHelper htmlHelper, 
			string componentName,
			T props,
			string path = null,
			string htmlTag = null,
			string containerId = null,
			bool clientOnly = false,
			bool serverOnly = false,
			string containerClass = null,
			Action<HttpResponse, RoutingContext> contextHandler = null
		)
		{
			try
			{
				var response = htmlHelper.ViewContext.HttpContext.Response;
				path = path ?? htmlHelper.ViewContext.HttpContext.Request.Path;

				var reactComponent 
					= Environment.CreateRouterComponent(
						componentName, 
						props, 
						path, 
						containerId, 
						clientOnly
					);

				if (!string.IsNullOrEmpty(htmlTag))
				{
					reactComponent.ContainerTag = htmlTag;
				}
				if (!string.IsNullOrEmpty(containerClass))
				{
					reactComponent.ContainerClass = containerClass;
				}
				
				var executionResult = reactComponent.RenderRouterWithContext(clientOnly, serverOnly);

				if (executionResult.Context?.status != null)
				{
					// Use provided contextHandler
					if (contextHandler != null)
					{
						contextHandler(response, executionResult.Context);
					}
					// Handle routing context internally
					else
					{
						SetServerResponse.ModifyResponse(executionResult.Context, response);
					}
				}

				return new HtmlString(executionResult.RenderResult);
			}
			finally
			{
				Environment.ReturnEngineToPool();
			}
		}
	}
}
