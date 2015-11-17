/*
 *  Copyright (c) 2014-2015, Facebook, Inc.
 *  All rights reserved.
 *
 *  This source code is licensed under the BSD-style license found in the
 *  LICENSE file in the root directory of this source tree. An additional grant
 *  of patent rights can be found in the PATENTS file in the same directory.
 */

using React.Exceptions;
using React.TinyIoC;

#if LEGACYASPNET
using System.Web;
using System.Web.Mvc;
using IHtmlHelper = System.Web.Mvc.HtmlHelper;
#else
using Microsoft.AspNet.Mvc.Rendering;
using IHtmlString = Microsoft.AspNet.Html.Abstractions.IHtmlContent;
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
				try
				{
					return ReactEnvironment.Current;
				}
				catch (TinyIoCResolutionException ex)
				{
					throw new ReactNotInitialisedException(
#if LEGACYASPNET
						"ReactJS.NET has not been initialised correctly.",
#else
						"ReactJS.NET has not been initialised correctly. Please ensure you have " +
						"called app.AddReact() and app.UseReact() in your Startup.cs file.",
#endif
						ex
					);
				}
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
		/// <param name="serverOnly">Skip rendering React specific data-attributes during server side rendering. Defaults to <c>false</c></param>
		/// <returns>The component's HTML</returns>
		public static IHtmlString React<T>(
			this IHtmlHelper htmlHelper,
			string componentName,
			T props,
			string htmlTag = null,
			string containerId = null,
			bool clientOnly = false,
			bool serverOnly = false
		)
		{
			var reactComponent = Environment.CreateComponent(componentName, props, containerId);
			if (!string.IsNullOrEmpty(htmlTag))
			{
				reactComponent.ContainerTag = htmlTag;
			}
			var result = reactComponent.RenderHtml(clientOnly, serverOnly);
			return new HtmlString(result);
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
		/// <returns>The component's HTML</returns>
		public static IHtmlString ReactWithInit<T>(
			this IHtmlHelper htmlHelper,
			string componentName,
			T props,
			string htmlTag = null,
			string containerId = null,
            bool clientOnly = false
		)
		{
			var reactComponent = Environment.CreateComponent(componentName, props, containerId);
			if (!string.IsNullOrEmpty(htmlTag))
			{
				reactComponent.ContainerTag = htmlTag;
			}
			var html = reactComponent.RenderHtml(clientOnly);

#if LEGACYASPNET
			var script = new TagBuilder("script")
			{
				InnerHtml = reactComponent.RenderJavaScript()
			};
#else
			var script = new TagBuilder("script");
			script.InnerHtml.AppendEncoded(reactComponent.RenderJavaScript());
#endif
			return new HtmlString(html + System.Environment.NewLine + script.ToString());
		}

		/// <summary>
		/// Renders the JavaScript required to initialise all components client-side. This will
		/// attach event handlers to the server-rendered HTML.
		/// </summary>
		/// <returns>JavaScript for all components</returns>
		public static IHtmlString ReactInitJavaScript(this IHtmlHelper htmlHelper)
		{
			var script = Environment.GetInitJavaScript();
#if LEGACYASPNET
			var tag = new TagBuilder("script")
			{
				InnerHtml = script
			};
			return new HtmlString(tag.ToString());
#else
			var tag = new TagBuilder("script");
			tag.InnerHtml.AppendEncoded(script);
			return tag;
#endif
		}
	}
}
