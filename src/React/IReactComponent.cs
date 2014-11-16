/*
 *  Copyright (c) 2014, Facebook, Inc.
 *  All rights reserved.
 *
 *  This source code is licensed under the BSD-style license found in the
 *  LICENSE file in the root directory of this source tree. An additional grant 
 *  of patent rights can be found in the PATENTS file in the same directory.
 */

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
		/// Renders the HTML for this component. This will execute the component server-side and
		/// return the rendered HTML.
		/// </summary>
		/// <returns>HTML</returns>
		string RenderHtml();

		/// <summary>
		/// Renders the JavaScript required to initialise this component client-side. This will 
		/// initialise the React component, which includes attach event handlers to the 
		/// server-rendered HTML.
		/// </summary>
		/// <returns>JavaScript</returns>
		string RenderJavaScript();
	}
}