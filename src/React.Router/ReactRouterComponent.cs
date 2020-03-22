/*
 * Copyright (c) Facebook, Inc. and its affiliates.
 *
 * This source code is licensed under the MIT license found in the
 * LICENSE file in the root directory of this source tree.
 */

using System.IO;
using Newtonsoft.Json;
using React.RenderFunctions;

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
		/// <param name="reactIdGenerator">React Id generator.</param>
		/// <param name="componentName">Name of the component.</param>
		/// <param name="containerId">The ID of the container DIV for this component</param>
		/// <param name="path">F.x. from Request.Path. Used by React Static Router to determine context and routing.</param>
		public ReactRouterComponent(
			IReactEnvironment environment,
			IReactSiteConfiguration configuration,
			IReactIdGenerator reactIdGenerator,
			string componentName,
			string containerId,
			string path
		) : base(environment, configuration, reactIdGenerator, componentName, containerId)
		{
			_path = path;
		}

		/// <summary>
		/// Render a React StaticRouter Component with context object.
		/// </summary>
		/// <param name="renderContainerOnly">Only renders component container. Used for client-side only rendering. Does not make sense in this context but included for consistency</param>
		/// <param name="renderServerOnly">Only renders the common HTML mark up and not any React specific data attributes. Used for server-side only rendering.</param>
		/// <param name="renderFunctions">Functions to call during component render</param>
		/// <returns>Object containing HTML in string format and the React Router context object</returns>
		public virtual ExecutionResult RenderRouterWithContext(
			bool renderContainerOnly = false,
			bool renderServerOnly = false,
			IRenderFunctions renderFunctions = null
		)
		{
			var reactRouterFunctions = new ReactRouterFunctions();

			var html = RenderHtml(
				renderContainerOnly,
				renderServerOnly,
				renderFunctions: new ChainedRenderFunctions(renderFunctions, reactRouterFunctions)
			);

			return new ExecutionResult
			{
				RenderResult = html,
				Context = JsonConvert.DeserializeObject<RoutingContext>(reactRouterFunctions.ReactRouterContext),
			};
		}

		/// <summary>
		/// Gets the JavaScript code to initialise the component
		/// </summary>
		/// <returns>JavaScript for component initialisation</returns>
		protected override void WriteComponentInitialiser(TextWriter writer)
		{
			writer.Write("React.createElement(");
			writer.Write(ComponentName);
			writer.Write(", Object.assign(");
			writer.Write(_serializedProps);
			writer.Write(", { location: ");
			writer.Write(JsonConvert.SerializeObject(
					_path,
					_configuration.JsonSerializerSettings));
			writer.Write(", context: context }))");
		}

		/// <summary>
		/// Renders the JavaScript required to initialise this component client-side. This will
		/// initialise the React component, which includes attach event handlers to the
		/// server-rendered HTML.
		/// Uses <see cref="ReactComponent"/> base Component initialiser.
		/// Client side React Router does not need context nor explicit path parameter.
		/// </summary>
		/// <returns>JavaScript</returns>
		public override void RenderJavaScript(TextWriter writer, bool waitForDOMContentLoad)
		{
			if (waitForDOMContentLoad)
			{
				writer.Write("window.addEventListener('DOMContentLoaded', function() {");
			}

			writer.Write("ReactDOM.hydrate(");
			base.WriteComponentInitialiser(writer);
			writer.Write(", document.getElementById(\"");
			writer.Write(ContainerId);
			writer.Write("\"))");

			if (waitForDOMContentLoad)
			{
				writer.Write("});");
			}
		}
	}
}
