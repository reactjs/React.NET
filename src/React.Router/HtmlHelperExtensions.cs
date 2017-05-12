using System;
using React.Exceptions;
using React.TinyIoC;

#if NET451
using System.Web;
using System.Web.Mvc;
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
				try
				{
					return ReactEnvironment.Current;
				}
				catch (TinyIoCResolutionException ex)
				{
					throw new ReactNotInitialisedException(
#if NET451
						"ReactJS.NET has not been initialised correctly.",
#else
						"ReactJS.NET has not been initialised correctly. Please ensure you have " +
						"called services.AddReact() and app.UseReact() in your Startup.cs file.",
#endif
						ex
					);
				}
			}
		}

		/// <summary>
		/// Render a React StaticRouter Component with context object.
		/// Can optionally be provided with a custom context handler to handle the various status codes.
		/// 
		/// </summary>
		/// <param name="htmlHelper">MVC Razor <see cref="IHtmlHelper"/></param>
		/// <param name="componentName">Name of React Static Router component. Expose component globally to ReactJS.NET</param>
		/// <param name="props">Props to initialise the component with</param>
		/// <param name="path">F.x. from Request.Url.AbsolutePath. Used by React Static Router to determine context and routing.</param>
		/// <param name="Response">Used either by contextHandler or internally to modify the Response status code and redirect.</param>
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
			HttpResponse Response = null,
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
				path = path ?? htmlHelper.ViewContext.HttpContext.Request.Path;
				Response = Response ?? htmlHelper.ViewContext.HttpContext.Response;

				var reactComponent = Environment.CreateRouterComponent(componentName, props, containerId, clientOnly);
				if (!string.IsNullOrEmpty(htmlTag))
				{
					reactComponent.ContainerTag = htmlTag;
				}
				if (!string.IsNullOrEmpty(containerClass))
				{
					reactComponent.ContainerClass = containerClass;
				}
				
				var executionResult = reactComponent.RenderRouterWithContext(path, clientOnly, serverOnly);

				if (executionResult.context?.status != null)
				{
					// Use provided contextHandler
					if (contextHandler != null)
					{
						contextHandler(Response, executionResult.context);
					}
					// Handle routing context internally
					else
					{
						HandleRoutingContext(executionResult.context, Response);
					}
				}

				return new HtmlString(executionResult.renderResult);
			}
			finally
			{
				Environment.ReturnEngineToPool();
			}
		}

		private static void HandleRoutingContext(RoutingContext context, HttpResponse Response)
		{
			var statusCode = context.status.Value;

			// 300-399
			if (statusCode >= 300 && statusCode < 400)
			{
				if (!string.IsNullOrEmpty(context.url))
				{
					if (statusCode == 301)
					{

#if NET451

						Response.RedirectPermanent(context.url);
#else
						Response.Redirect(context.url, true);
#endif
					}
					else // 302 and all others
					{
						Response.Redirect(context.url);
					}
				}
				else
				{
					throw new ReactRouterException("Router requested redirect but no url provided.");
				}
			}
			else
			{
				Response.StatusCode = statusCode;
			}
		}
	}
}
