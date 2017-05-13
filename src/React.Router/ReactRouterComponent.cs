/*
 *  Copyright (c) 2014-Present, Facebook, Inc.
 *  All rights reserved.
 *
 *  This source code is licensed under the BSD-style license found in the
 *  LICENSE file in the root directory of this source tree. An additional grant
 *  of patent rights can be found in the PATENTS file in the same directory.
 */

 using JavaScriptEngineSwitcher.Core;
using Newtonsoft.Json;
using React.Exceptions;

namespace React.Router
{
	/// <summary>
	/// Represents a React Router JavaScript component.
	/// </summary>
	public class ReactRouterComponent : ReactComponent
	{
		/// <summary>
		/// F.x. from Request.Path. Used by React Static Router to determine context and routing.
		/// </summary>
		protected string _path;

		/// <summary>
		/// Initialises a new instance of the <see cref="ReactRouterComponent"/> class.
		/// </summary>
		/// <param name="environment">The environment.</param>
		/// <param name="configuration">Site-wide configuration.</param>
		/// <param name="componentName">Name of the component.</param>
		/// <param name="containerId">The ID of the container DIV for this component</param>
		/// <param name="path">F.x. from Request.Path. Used by React Static Router to determine context and routing.</param>
		public ReactRouterComponent(
			IReactEnvironment environment,
			IReactSiteConfiguration configuration,
			string componentName,
			string containerId,
			string path
		) : base(environment, configuration, componentName, containerId)
		{
			_path = path;
		}

		/// <summary>
		/// Render a React StaticRouter Component with context object.
		/// </summary>
		/// <param name="renderContainerOnly">Only renders component container. Used for client-side only rendering. Does not make sense in this context but included for consistency</param>
		/// <param name="renderServerOnly">Only renders the common HTML mark up and not any React specific data attributes. Used for server-side only rendering.</param>
		/// <returns>Object containing HTML in string format and the React Router context object</returns>
		public virtual ExecutionResult RenderRouterWithContext(bool renderContainerOnly = false, bool renderServerOnly = false)
		{
			if (!EngineSupportsObjectAssign())
			{
				_environment.Execute(objectAssignPolyfill);
			}

			_environment.Execute("var context = {};");

			var html = RenderHtml(renderContainerOnly, renderServerOnly);

			var contextString = _environment.Execute<string>("JSON.stringify(context);");

			return new ExecutionResult
			{
				RenderResult = html,
				Context = JsonConvert.DeserializeObject<RoutingContext>(contextString),
			};
		}

		/// <summary>
		/// Gets the JavaScript code to initialise the component
		/// </summary>
		/// <returns>JavaScript for component initialisation</returns>
		protected override string GetComponentInitialiser()
		{
			return string.Format(
				@"React.createElement
					({0}, Object.assign({1}, {{ path: '{2}', context: context }}))",
				ComponentName,
				_serializedProps,
				_path
			);
		}

		/// <summary>
		/// Renders the JavaScript required to initialise this component client-side. This will
		/// initialise the React component, which includes attach event handlers to the
		/// server-rendered HTML.
		/// Uses <see cref="ReactComponent"/> base Component initialiser.
		/// Client side React Router does not need context nor explicit path parameter.
		/// </summary>
		/// <returns>JavaScript</returns>
		public override string RenderJavaScript()
		{
			return string.Format(
				"ReactDOM.render({0}, document.getElementById({1}))",
				base.GetComponentInitialiser(),
				JsonConvert.SerializeObject(ContainerId, _configuration.JsonSerializerSettings) // SerializeObject accepts null settings
			);
		}

		/// <summary>
		/// Ensure js engine supports Object.assign
		/// </summary>
		public virtual bool EngineSupportsObjectAssign()
		{
			return _environment.Execute<bool>(
				"typeof Object.assign === 'function'"
			);
		}

		/// <summary>
		/// Polyfill for engines that do not support Object.assign
		/// </summary>
		protected static string objectAssignPolyfill =
			@"Object.assign = function(target, varArgs) { // .length of function is 2
				'use strict';
				if (target == null) { // TypeError if undefined or null
				  throw new TypeError('Cannot convert undefined or null to object');
				}

				var to = Object(target);

				for (var index = 1; index < arguments.length; index++) {
				  var nextSource = arguments[index];

				  if (nextSource != null) { // Skip over if undefined or null
					for (var nextKey in nextSource) {
					  // Avoid bugs when hasOwnProperty is shadowed
					  if (Object.prototype.hasOwnProperty.call(nextSource, nextKey)) {
						to[nextKey] = nextSource[nextKey];
					  }
					}
				  }
				}
				return to;
			  };
			";
	}
}
