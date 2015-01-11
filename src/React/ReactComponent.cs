﻿/*
 *  Copyright (c) 2014-2015, Facebook, Inc.
 *  All rights reserved.
 *
 *  This source code is licensed under the BSD-style license found in the
 *  LICENSE file in the root directory of this source tree. An additional grant 
 *  of patent rights can be found in the PATENTS file in the same directory.
 */

using System.Linq;
using System.Text.RegularExpressions;
using JavaScriptEngineSwitcher.Core;
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
		/// Regular expression used to validate JavaScript identifiers. Used to ensure component
		/// names are valid.
		/// Based off https://gist.github.com/Daniel15/3074365
		/// </summary>
		private static readonly Regex _identifierRegex = new Regex(@"^[a-zA-Z_$][0-9a-zA-Z_$]*(?:\[(?:"".+""|\'.+\'|\d+)\])*?$", RegexOptions.Compiled);

		/// <summary>
		/// Environment this component has been created in
		/// </summary>
		protected readonly IReactEnvironment _environment;

		/// <summary>
		/// Global site configuration
		/// </summary>
		protected readonly IReactSiteConfiguration _configuration;

		/// <summary>
		/// Gets or sets the name of the component
		/// </summary>
		public string ComponentName { get; set; }

		/// <summary>
		/// Gets or sets the unique ID for the DIV container of this component
		/// </summary>
		public string ContainerId { get; set; }

		/// <summary>
		/// Gets or sets the HTML tag the component is wrapped in
		/// </summary>
		public string ContainerTag { get; set; }

		/// <summary>
		/// Gets or sets the props for this component
		/// </summary>
		public object Props { get; set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="ReactComponent"/> class.
		/// </summary>
		/// <param name="environment">The environment.</param>
		/// <param name="configuration">Site-wide configuration.</param>
		/// <param name="componentName">Name of the component.</param>
		/// <param name="containerId">The ID of the container DIV for this component</param>
		public ReactComponent(IReactEnvironment environment, IReactSiteConfiguration configuration, string componentName, string containerId)
		{
			EnsureComponentNameValid(componentName);
			_environment = environment;
			_configuration = configuration;
			ComponentName = componentName;
			ContainerId = containerId;
			ContainerTag = "div";
		}

		/// <summary>
		/// Renders the HTML for this component. This will execute the component server-side and
		/// return the rendered HTML.
		/// </summary>
		/// <returns>HTML</returns>
		public virtual string RenderHtml()
		{
			EnsureComponentExists();
			try
			{
				var html = _environment.Execute<string>(
					string.Format("React.renderToString({0})", GetComponentInitialiser())
					);
				return string.Format(
					"<{2} id=\"{0}\">{1}</{2}>",
					ContainerId,
					html,
					ContainerTag
				);
			}
			catch (JsRuntimeException ex)
			{
				throw new ReactServerRenderingException(string.Format(
					"Error while rendering \"{0}\" to \"{2}\": {1}",
					ComponentName,
					ex.Message,
					ContainerId
				));
			}
		}

		/// <summary>
		/// Renders the JavaScript required to initialise this component client-side. This will 
		/// initialise the React component, which includes attach event handlers to the 
		/// server-rendered HTML.
		/// </summary>
		/// <returns>JavaScript</returns>
		public virtual string RenderJavaScript()
		{
			return string.Format(
				"React.render({0}, document.getElementById({1}))",
				GetComponentInitialiser(),
				JsonConvert.SerializeObject(ContainerId, _configuration.JsonSerializerSettings) // SerializeObject accepts null settings
			);
		}

		/// <summary>
		/// Ensures that this component exists in global scope
		/// </summary>
		protected virtual void EnsureComponentExists()
		{
			// This is safe as componentName was validated via EnsureComponentNameValid()
			var componentExists = _environment.Execute<bool>(string.Format(
				"typeof {0} !== 'undefined'",
				ComponentName
			));
			if (!componentExists)
			{
				throw new ReactInvalidComponentException(string.Format(
					"Could not find a component named '{0}'. Did you forget to add it to " +
					"App_Start\\ReactConfig.cs?",
					ComponentName
				));
			}
		}

		/// <summary>
		/// Gets the JavaScript code to initialise the component
		/// </summary>
		/// <returns>JavaScript for component initialisation</returns>
		protected virtual string GetComponentInitialiser()
		{
			var encodedProps = JsonConvert.SerializeObject(Props, _configuration.JsonSerializerSettings); // SerializeObject accepts null settings
			return string.Format(
				"React.createElement({0}, {1})",
				ComponentName,
				encodedProps
			);
		}

		/// <summary>
		/// Validates that the specified component name is valid
		/// </summary>
		/// <param name="componentName"></param>
		internal static void EnsureComponentNameValid(string componentName)
		{
			var isValid = componentName.Split('.').All(segment => _identifierRegex.IsMatch(segment));
			if (!isValid)
			{
				throw new ReactInvalidComponentException(string.Format(
					"Invalid component name '{0}'",
					componentName
				));
			}
		}
	}
}
