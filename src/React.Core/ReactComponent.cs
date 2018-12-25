/*
 * Copyright (c) Facebook, Inc. and its affiliates.
 *
 * This source code is licensed under the MIT license found in the
 * LICENSE file in the root directory of this source tree.
 */

using System;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Text;
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
		private static readonly ConcurrentDictionary<string, bool> _componentNameValidCache = new ConcurrentDictionary<string, bool>(StringComparer.Ordinal);

		[ThreadStatic]
		private static StringWriter _sharedStringWriter;

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
		/// Raw props for this component
		/// </summary>
		protected object _props;

		/// <summary>
		/// JSON serialized props for this component
		/// </summary>
		protected string _serializedProps;

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
		/// Gets or sets the HTML class for the container of this component
		/// </summary>
		public string ContainerClass { get; set; }

		/// <summary>
		/// Get or sets if this components only should be rendered server side
		/// </summary>
		public bool ServerOnly { get; set; }

		/// <summary>
		/// Gets or sets the props for this component
		/// </summary>
		public object Props
		{
			get { return _props; }
			set
			{
				_props = value;
				_serializedProps = JsonConvert.SerializeObject(
					value,
					_configuration.JsonSerializerSettings);
			}
		}
		/// <summary>
		/// Get or sets if this components only should be rendered client side
		/// </summary>
		public bool ClientOnly { get; set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="ReactComponent"/> class.
		/// </summary>
		/// <param name="environment">The environment.</param>
		/// <param name="configuration">Site-wide configuration.</param>
		/// <param name="reactIdGenerator">React Id generator.</param>
		/// <param name="componentName">Name of the component.</param>
		/// <param name="containerId">The ID of the container DIV for this component</param>
		public ReactComponent(IReactEnvironment environment, IReactSiteConfiguration configuration, IReactIdGenerator reactIdGenerator, string componentName, string containerId)
		{
			EnsureComponentNameValid(componentName);
			_environment = environment;
			_configuration = configuration;
			ComponentName = componentName;
			ContainerId = string.IsNullOrEmpty(containerId) ? reactIdGenerator.Generate() : containerId;
			ContainerTag = "div";
		}

		/// <summary>
		/// Renders the HTML for this component. This will execute the component server-side and
		/// return the rendered HTML.
		/// </summary>
		/// <param name="renderContainerOnly">Only renders component container. Used for client-side only rendering.</param>
		/// <param name="renderServerOnly">Only renders the common HTML mark up and not any React specific data attributes. Used for server-side only rendering.</param>
		/// <param name="exceptionHandler">A custom exception handler that will be called if a component throws during a render. Args: (Exception ex, string componentName, string containerId)</param>
		/// <param name="renderFunctions">Functions to call during component render</param>
		/// <returns>HTML</returns>
		public virtual string RenderHtml(bool renderContainerOnly = false, bool renderServerOnly = false, Action<Exception, string, string> exceptionHandler = null, IRenderFunctions renderFunctions = null)
		{
			return GetStringFromWriter(renderHtmlWriter => RenderHtml(renderHtmlWriter, renderContainerOnly, renderServerOnly, exceptionHandler, renderFunctions));
		}

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
		public virtual void RenderHtml(TextWriter writer, bool renderContainerOnly = false, bool renderServerOnly = false, Action<Exception, string, string> exceptionHandler = null, IRenderFunctions renderFunctions = null)
		{
			if (!_configuration.UseServerSideRendering)
			{
				renderContainerOnly = true;
			}

			if (!renderContainerOnly)
			{
				EnsureComponentExists();
			}

			var html = string.Empty;
			if (!renderContainerOnly)
			{
				var stringWriter = _sharedStringWriter;
				if (stringWriter != null)
				{
					stringWriter.GetStringBuilder().Clear();
				}
				else
				{
					_sharedStringWriter = stringWriter = new StringWriter(new StringBuilder(_serializedProps.Length + 128));
				}

				try
				{
					stringWriter.Write(renderServerOnly ? "ReactDOMServer.renderToStaticMarkup(" : "ReactDOMServer.renderToString(");
					if (renderFunctions != null)
					{
						stringWriter.Write(renderFunctions.WrapComponent(GetStringFromWriter(componentInitWriter => WriteComponentInitialiser(componentInitWriter))));
					}
					else
					{
						WriteComponentInitialiser(stringWriter);
					}
					stringWriter.Write(')');

					if (renderFunctions != null)
					{
						renderFunctions.PreRender(x => _environment.Execute<string>(x));
						html = _environment.Execute<string>(renderFunctions.TransformRenderedHtml(stringWriter.ToString()));
						renderFunctions.PostRender(x => _environment.Execute<string>(x));
					}
					else
					{
						html = _environment.Execute<string>(stringWriter.ToString());
					}

					if (renderServerOnly)
					{
						writer.Write(html);
						return;
					}
				}
				catch (JsRuntimeException ex)
				{
					if (exceptionHandler == null)
					{
						exceptionHandler = _configuration.ExceptionHandler;
					}

					exceptionHandler(ex, ComponentName, ContainerId);
				}
			}

			writer.Write('<');
			writer.Write(ContainerTag);
			writer.Write(" id=\"");
			writer.Write(ContainerId);
			writer.Write('"');
			if (!string.IsNullOrEmpty(ContainerClass))
			{
				writer.Write(" class=\"");
				writer.Write(ContainerClass);
				writer.Write('"');
			}

			writer.Write('>');
			writer.Write(html);
			writer.Write("</");
			writer.Write(ContainerTag);
			writer.Write('>');
		}

		/// <summary>
		/// Renders the JavaScript required to initialise this component client-side. This will
		/// initialise the React component, which includes attach event handlers to the
		/// server-rendered HTML.
		/// </summary>
		/// <returns>JavaScript</returns>
		public virtual string RenderJavaScript()
		{
			return GetStringFromWriter(renderJsWriter => RenderJavaScript(renderJsWriter));
		}

		/// <summary>
		/// Renders the JavaScript required to initialise this component client-side. This will
		/// initialise the React component, which includes attach event handlers to the
		/// server-rendered HTML.
		/// </summary>
		/// <param name="writer">The <see cref="T:System.IO.TextWriter" /> to which the content is written</param>
		/// <returns>JavaScript</returns>
		public virtual void RenderJavaScript(TextWriter writer)
		{
			writer.Write(
				!_configuration.UseServerSideRendering || ClientOnly ? "ReactDOM.render(" : "ReactDOM.hydrate(");
			WriteComponentInitialiser(writer);
			writer.Write(", document.getElementById(\"");
			writer.Write(ContainerId);
			writer.Write("\"))");
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
		/// <param name="writer">The <see cref="T:System.IO.TextWriter" /> to which the content is written</param>
		protected virtual void WriteComponentInitialiser(TextWriter writer)
		{
			writer.Write("React.createElement(");
			writer.Write(ComponentName);
			writer.Write(", ");
			writer.Write(_serializedProps);
			writer.Write(')');
		}

		/// <summary>
		/// Validates that the specified component name is valid
		/// </summary>
		/// <param name="componentName"></param>
		internal static void EnsureComponentNameValid(string componentName)
		{
			var isValid = _componentNameValidCache.GetOrAdd(componentName, compName => compName.Split('.').All(segment => _identifierRegex.IsMatch(segment)));
			if (!isValid)
			{
				throw new ReactInvalidComponentException($"Invalid component name '{componentName}'");
			}
		}

		private string GetStringFromWriter(Action<TextWriter> fnWithTextWriter)
		{
			using (var textWriter = new StringWriter())
			{
				fnWithTextWriter(textWriter);
				return textWriter.ToString();
			}
		}
	}
}
