/*
 * Copyright (c) Facebook, Inc. and its affiliates.
 *
 * This source code is licensed under the MIT license found in the
 * LICENSE file in the root directory of this source tree.
 */

using System;
using System.IO;

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
		/// <param name="renderFunctions">Functions to call during component render</param>
		/// <returns>HTML</returns>
		string RenderHtml(bool renderContainerOnly = false, bool renderServerOnly = false, Action<Exception, string, string> exceptionHandler = null, IRenderFunctions renderFunctions = null);

		/// <summary>
		/// Renders the HTML for this component. This will execute the component server-side and
		/// return the rendered HTML.
		/// </summary>
		/// <param name="writer">The <see cref="T:System.IO.TextWriter" /> to which the content is written</param>
		/// <param name="renderContainerOnly">Only renders component container. Used for client-side only rendering.</param>
		/// <param name="renderServerOnly">Only renders the common HTML mark up and not any React specific data attributes. Used for server-side only rendering.</param>
		/// <param name="exceptionHandler">A custom exception handler that will be called if a component throws during a render. Args: (Exception ex, string componentName, string containerId)</param>
		/// <param name="renderFunctions">Functions to call during component render</param>
		/// <returns>HTML</returns>
		void RenderHtml(TextWriter writer, bool renderContainerOnly = false, bool renderServerOnly = false, Action<Exception, string, string> exceptionHandler = null, IRenderFunctions renderFunctions = null);

		/// <summary>
		/// Renders the JavaScript required to initialise this component client-side. This will
		/// initialise the React component, which includes attach event handlers to the
		/// server-rendered HTML.
		/// </summary>
		/// <returns>JavaScript</returns>
		string RenderJavaScript(bool waitForDOMContentLoad);

		/// <summary>
		/// Renders the JavaScript required to initialise this component client-side. This will
		/// initialise the React component, which includes attach event handlers to the
		/// server-rendered HTML.
		/// </summary>
		/// <returns>JavaScript</returns>
		void RenderJavaScript(TextWriter writer, bool waitForDOMContentLoad);
	}
}
