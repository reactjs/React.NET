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
using System.Web.Mvc;
using IHtmlHelper = System.Web.Mvc.HtmlHelper;
#else
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Mvc.Rendering;
using IHtmlString = Microsoft.AspNetCore.Html.IHtmlContent;
using Microsoft.AspNetCore.Html;
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
				var result = reactComponent.RenderHtml(clientOnly, serverOnly, exceptionHandler);
				return new HtmlString(result);
			}
			finally
			{
				Environment.ReturnEngineToPool();
			}
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
				var html = reactComponent.RenderHtml(clientOnly, exceptionHandler: exceptionHandler);

				return new HtmlString(html + System.Environment.NewLine + RenderToString(GetScriptTag(reactComponent.RenderJavaScript())));
			}
			finally
			{
				Environment.ReturnEngineToPool();
			}
		}

		/// <summary>
		/// Renders the JavaScript required to initialise all components client-side. This will
		/// attach event handlers to the server-rendered HTML.
		/// </summary>
		/// <returns>JavaScript for all components</returns>
		public static IHtmlString ReactInitJavaScript(this IHtmlHelper htmlHelper, bool clientOnly = false)
		{
			try
			{
				return GetScriptTag(Environment.GetInitJavaScript(clientOnly));
			}
			finally
			{
				Environment.ReturnEngineToPool();
			}
		}

		private static IHtmlString GetScriptTag(string script)
		{
#if LEGACYASPNET
			var tag = new TagBuilder("script")
			{
				InnerHtml = script,
			};

			if (Environment.Configuration.ScriptNonceProvider != null)
			{
				tag.Attributes.Add("nonce", Environment.Configuration.ScriptNonceProvider());
			}

			return new HtmlString(tag.ToString());
#else
			var tag = new TagBuilder("script");
			tag.InnerHtml.AppendHtml(script);

			if (Environment.Configuration.ScriptNonceProvider != null)
			{
				tag.Attributes.Add("nonce", Environment.Configuration.ScriptNonceProvider());
			}

			return tag;
#endif
		}

		// In ASP.NET Core, you can no longer call `.ToString` on `IHtmlString`
		private static string RenderToString(IHtmlString source)
		{
#if LEGACYASPNET
			return source.ToString();
#else
			using (var writer = new StringWriter())
			{
				source.WriteTo(writer, HtmlEncoder.Default);
				return writer.ToString();
			}
#endif
		}
	}
}
