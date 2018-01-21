/*
 *  Copyright (c) 2014-Present, Facebook, Inc.
 *  All rights reserved.
 *
 *  This source code is licensed under the BSD-style license found in the
 *  LICENSE file in the root directory of this source tree. An additional grant
 *  of patent rights can be found in the PATENTS file in the same directory.
 */

using System;

namespace React
{
	/// <summary>
	/// Represents a React JavaScript component.
	/// </summary>
	public interface IReactComponent
	{
		/// <summary>
		/// Gets or sets the props for this component
		/// </summary>
		object Props { get; set; }

		/// <summary>
		/// Gets or sets the name of the component
		/// </summary>
		string ComponentName { get; set; }

		/// <summary>
		/// Gets or sets the unique ID for the container of this component
		/// </summary>
		string ContainerId { get; set; }

		/// <summary>
		/// Gets or sets the HTML tag the component is wrapped in
		/// </summary>
		string ContainerTag { get; set; }

		/// <summary>
		/// Gets or sets the HTML class for the container of this component
		/// </summary>
		string ContainerClass { get; set; }

		/// <summary>
		/// Get or sets if this components only should be rendered server side
		/// </summary>
		bool ServerOnly { get; set; }

		/// <summary>
		/// Renders the HTML for this component. This will execute the component server-side and
		/// return the rendered HTML.
		/// </summary>
		/// <param name="renderContainerOnly">Only renders component container. Used for client-side only rendering.</param>
		/// <param name="renderServerOnly">Only renders the common HTML mark up and not any React specific data attributes. Used for server-side only rendering.</param>
		/// <param name="exceptionHandler">A custom exception handler that will be called if a component throws during a render. Args: (Exception ex, string componentName, string containerId)</param>
		/// <returns>HTML</returns>
		string RenderHtml(bool renderContainerOnly = false, bool renderServerOnly = false, Action<Exception, string, string> exceptionHandler = null);

		/// <summary>
		/// Renders the JavaScript required to initialise this component client-side. This will
		/// initialise the React component, which includes attach event handlers to the
		/// server-rendered HTML.
		/// </summary>
		/// <returns>JavaScript</returns>
		string RenderJavaScript();
	}
}
