/*
 *  Copyright (c) 2014-Present, Facebook, Inc.
 *  All rights reserved.
 *
 *  This source code is licensed under the BSD-style license found in the
 *  LICENSE file in the root directory of this source tree. An additional grant
 *  of patent rights can be found in the PATENTS file in the same directory.
 */

using System;
using System.IO;

#if LEGACYASPNET
using System.Web;
using IHtmlHelper = System.Web.Mvc.HtmlHelper;
#else
using Microsoft.AspNetCore.Mvc.Rendering;
using IHtmlString = Microsoft.AspNetCore.Html.IHtmlContent;
#endif

#if LEGACYASPNET
namespace React.Web.Mvc
#else
namespace React.AspNet
#endif
{
	/// <summary>
	/// HTML Helpers for utilising React from an ASP.NET MVC application.
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
		/// Renders the specified React component
		/// </summary>
		/// <typeparam name="T">Type of the props</typeparam>
		/// <param name="htmlHelper">HTML helper</param>
		/// <param name="componentName">Name of the component</param>
		/// <param name="props">Props to initialise the component with</param>
		/// <param name="htmlTag">HTML tag to wrap the component in. Defaults to &lt;div&gt;</param>
		/// <param name="containerId">ID to use for the container HTML tag. Defaults to an auto-generated ID</param>
		/// <param name="clientOnly">Skip rendering server-side and only output client-side initialisation code. Defaults to <c>false</c></param>
		/// <param name="serverOnly">Skip rendering React specific data-attributes, container and client-side initialisation during server side rendering. Defaults to <c>false</c></param>
		/// <param name="containerClass">HTML class(es) to set on the container tag</param>
		/// <param name="exceptionHandler">A custom exception handler that will be called if a component throws during a render. Args: (Exception ex, string componentName, string containerId)</param>
		/// <returns>The component's HTML</returns>
		public static IHtmlString React<T>(
			this IHtmlHelper htmlHelper,
			string componentName,
			T props,
			string htmlTag = null,
			string containerId = null,
			bool clientOnly = false,
			bool serverOnly = false,
			string containerClass = null,
			Action<Exception, string, string> exceptionHandler = null
		)
		{
			return new ActionHtmlString(writer =>
			{
				try
				{
					var reactComponent = Environment.CreateComponent(componentName, props, containerId, clientOnly, serverOnly);
					if (!string.IsNullOrEmpty(htmlTag))
					{
						reactComponent.ContainerTag = htmlTag;
					}

					if (!string.IsNullOrEmpty(containerClass))
					{
						reactComponent.ContainerClass = containerClass;
					}

					writer.Write(reactComponent.RenderHtml(clientOnly, serverOnly, exceptionHandler));
				}
				finally
				{
					Environment.ReturnEngineToPool();
				}
			});
		}

		/// <summary>
		/// Renders the specified React component, along with its client-side initialisation code.
		/// Normally you would use the <see cref="React{T}"/> method, but <see cref="ReactWithInit{T}"/>
		/// is useful when rendering self-contained partial views.
		/// </summary>
		/// <typeparam name="T">Type of the props</typeparam>
		/// <param name="htmlHelper">HTML helper</param>
		/// <param name="componentName">Name of the component</param>
		/// <param name="props">Props to initialise the component with</param>
		/// <param name="htmlTag">HTML tag to wrap the component in. Defaults to &lt;div&gt;</param>
		/// <param name="containerId">ID to use for the container HTML tag. Defaults to an auto-generated ID</param>
		/// <param name="clientOnly">Skip rendering server-side and only output client-side initialisation code. Defaults to <c>false</c></param>
		/// <param name="containerClass">HTML class(es) to set on the container tag</param>
		/// <param name="exceptionHandler">A custom exception handler that will be called if a component throws during a render. Args: (Exception ex, string componentName, string containerId)</param>
		/// <returns>The component's HTML</returns>
		public static IHtmlString ReactWithInit<T>(
			this IHtmlHelper htmlHelper,
			string componentName,
			T props,
			string htmlTag = null,
			string containerId = null,
			bool clientOnly = false,
			string containerClass = null,
			Action<Exception, string, string> exceptionHandler = null
		)
		{
			return new ActionHtmlString(writer =>
			{
				try
				{
					var reactComponent = Environment.CreateComponent(componentName, props, containerId, clientOnly);
					if (!string.IsNullOrEmpty(htmlTag))
					{
						reactComponent.ContainerTag = htmlTag;
					}

					if (!string.IsNullOrEmpty(containerClass))
					{
						reactComponent.ContainerClass = containerClass;
					}

					writer.Write(reactComponent.RenderHtml(clientOnly, exceptionHandler: exceptionHandler));
					writer.WriteLine();
					WriteScriptTag(writer, bodyWriter => bodyWriter.Write(reactComponent.RenderJavaScript()));
				}
				finally
				{
					Environment.ReturnEngineToPool();
				}
			});
		}

		/// <summary>
		/// Renders the JavaScript required to initialise all components client-side. This will
		/// attach event handlers to the server-rendered HTML.
		/// </summary>
		/// <returns>JavaScript for all components</returns>
		public static IHtmlString ReactInitJavaScript(this IHtmlHelper htmlHelper, bool clientOnly = false)
		{
			return new ActionHtmlString(writer =>
			{
				try
				{
					WriteScriptTag(writer, bodyWriter => bodyWriter.Write(Environment.GetInitJavaScript(clientOnly)));
				}
				finally
				{
					Environment.ReturnEngineToPool();
				}
			});
		}

		private static void WriteScriptTag(TextWriter writer, Action<TextWriter> bodyWriter)
		{
			writer.Write("<script");
			if (Environment.Configuration.ScriptNonceProvider != null)
			{
				writer.Write(" nonce=\"");
				writer.Write(Environment.Configuration.ScriptNonceProvider());
				writer.Write("\"");
			}

			writer.Write(">");

			bodyWriter(writer);

			writer.Write("</script>");
		}
	}
}
