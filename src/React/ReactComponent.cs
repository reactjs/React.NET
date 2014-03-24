/*
 *  Copyright (c) 2014, Facebook, Inc.
 *  All rights reserved.
 *
 *  This source code is licensed under the BSD-style license found in the
 *  LICENSE file in the root directory of this source tree. An additional grant 
 *  of patent rights can be found in the PATENTS file in the same directory.
 */

using Newtonsoft.Json;
using React.Exceptions;

namespace React
{
	/// <summary>
	/// Represents a React JavaScript component.
	/// </summary>
	public class ReactComponent : IReactComponent
	{
		/// <summary>
		/// Environment this component has been created in
		/// </summary>
		private readonly IReactEnvironment _environment;

		/// <summary>
		/// Name of the component
		/// </summary>
		private readonly string _componentName;

		/// <summary>
		/// Unique ID for the DIV container of this component
		/// </summary>
		private readonly string _containerId;

		/// <summary>
		/// Gets or sets the props for this component
		/// </summary>
		public object Props { get; set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="ReactComponent"/> class.
		/// </summary>
		/// <param name="environment">The environment.</param>
		/// <param name="componentName">Name of the component.</param>
		/// <param name="containerId">The ID of the container DIV for this component</param>
		public ReactComponent(IReactEnvironment environment, string componentName, string containerId)
		{
			_environment = environment;
			_componentName = componentName;
			_containerId = containerId;
		}

		/// <summary>
		/// Renders the HTML for this component. This will execute the component server-side and
		/// return the rendered HTML.
		/// </summary>
		/// <returns>HTML</returns>
		public string RenderHtml()
		{
			EnsureComponentExists();
			var html = _environment.Execute<string>(
				string.Format("React.renderComponentToString({0})", GetComponentInitialiser())
			);
			// TODO: Allow changing of the wrapper tag element from a DIV to something else
			return string.Format(
				"<div id=\"{0}\">{1}</div>",
				_containerId,
				html
			);
		}

		/// <summary>
		/// Renders the JavaScript required to initialise this component client-side. This will 
		/// initialise the React component, which includes attach event handlers to the 
		/// server-rendered HTML.
		/// </summary>
		/// <returns>JavaScript</returns>
		public string RenderJavaScript()
		{
			return string.Format(
				"React.renderComponent({0}, document.getElementById({1}))",
				GetComponentInitialiser(),
				JsonConvert.SerializeObject(_containerId)
			);
		}

		/// <summary>
		/// Ensures that this component exists in global scope
		/// </summary>
		private void EnsureComponentExists()
		{
			if (!_environment.HasVariable(_componentName))
			{
				throw new ReactInvalidComponentException(string.Format(
					"Could not find a component named '{0}'. Did you forget to add it to " +
					"App_Start\\ReactConfig.cs?",
					_componentName
				));
			}
		}

		/// <summary>
		/// Gets the JavaScript code to initialise the component
		/// </summary>
		/// <returns>JavaScript for component initialisation</returns>
		private string GetComponentInitialiser()
		{
			var encodedProps = JsonConvert.SerializeObject(Props);
			return string.Format(
				"{0}({1})",
				_componentName,
				encodedProps
			);
		}
	}
}
