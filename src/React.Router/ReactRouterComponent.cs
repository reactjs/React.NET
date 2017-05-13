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
		/// Initializes a new instance of the <see cref="ReactRouterComponent"/> class.
		/// </summary>
		/// <param name="environment">The environment.</param>
		/// <param name="configuration">Site-wide configuration.</param>
		/// <param name="componentName">Name of the component.</param>
		/// <param name="containerId">The ID of the container DIV for this component</param>
		public ReactRouterComponent(
			IReactEnvironment environment,
			IReactSiteConfiguration configuration,
			string componentName,
			string containerId
		) : base(environment, configuration, componentName, containerId)
		{

		}

		/// <summary>
		/// <see cref="ReactRouterComponent"/>'s do not use this method.
		/// Instead use RenderRouterWithContext.
		/// </summary>
		/// <param name="renderContainerOnly"></param>
		/// <param name="renderServerOnly"></param>
		/// <returns></returns>
		public override string RenderHtml(bool renderContainerOnly = false, bool renderServerOnly = false)
		{
			throw new ReactRouterException
				(@"React Router Components are rendered with RenderRouterWithContext. 
					Please use ReactComponent if this functionality is not desired.");
		}

		/// <summary>
		/// Render a React StaticRouter Component with context object.
		/// </summary>
		/// <param name="path">Current request URL path. F.x. Request.Path</param>
		/// <param name="renderContainerOnly">Only renders component container. Used for client-side only rendering. Does not make sense in this context but included for consistency</param>
		/// <param name="renderServerOnly">Only renders the common HTML mark up and not any React specific data attributes. Used for server-side only rendering.</param>
		/// <returns>Object containing HTML in string format and the React Router context object</returns>
		public ExecutionResult RenderRouterWithContext(string path, bool renderContainerOnly = false, bool renderServerOnly = false)
		{
			if (!_configuration.UseServerSideRendering)
			{
				renderContainerOnly = true;
			}

			if (!renderContainerOnly)
			{
				EnsureComponentExists();
			}

			try
			{
				ExecutionResult executionResult;

				if (!renderContainerOnly)
				{
					var componentInitialiser = string.Format("React.createElement({0}, {{ path: '{1}', context: context }})", ComponentName, path );

					var reactDomServerMethod = renderServerOnly ? "renderToStaticMarkup" : "renderToString";

					var reactRenderCommand = string.Format(@"

						var context = {{}};

						var renderResult = ReactDOMServer.{0}(
							React.createElement({1}, {{ path: '{2}', context: context }})
						);

						JSON.stringify({{
							renderResult: renderResult,
							context: context
						}});
					", reactDomServerMethod, ComponentName, path);

					var strResult = _environment.Execute<string>(reactRenderCommand);
					executionResult = JsonConvert.DeserializeObject<ExecutionResult>(strResult);
				}
				else
				{
					executionResult = new ExecutionResult();
				}

				string attributes = string.Format("id=\"{0}\"", ContainerId);
				if (!string.IsNullOrEmpty(ContainerClass))
				{
					attributes += string.Format(" class=\"{0}\"", ContainerClass);
				}

				executionResult.renderResult =
					string.Format(
						"<{2} {0}>{1}</{2}>",
						attributes,
						executionResult.renderResult,
						ContainerTag
					);

				return executionResult;
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
	}
}
