using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Web;
using React.Exceptions;

namespace React
{
	/// <summary>
	/// Request-specific React.NET environment. This is unique to the individual request and is 
	/// not shared.
	/// TODO: This is probably not thread safe at all (especially JSXTransformer)
	/// </summary>
	public class ReactEnvironment : IReactEnvironment
	{
		/// <summary>
		/// Format string used for React component container IDs
		/// </summary>
		private const string CONTAINER_ELEMENT_NAME = "react{0}";

		/// <summary>
		/// The JavaScript engine used in this environment
		/// </summary>
		private readonly IJavascriptEngine _engine;
		/// <summary>
		/// Site-wide configuration
		/// </summary>
		private readonly IReactSiteConfiguration _config;
		/// <summary>
		/// Server utilities
		/// </summary>
		private readonly HttpServerUtilityBase _server;
		/// <summary>
		/// Number of components instantiated in this environment
		/// </summary>
		private int _maxContainerId = 0;
		/// <summary>
		/// List of all components instantiated in this environment
		/// </summary>
		private readonly IList<IReactComponent> _components = new List<IReactComponent>();
		/// <summary>
		/// Whether the JSX Transformer has been loaded
		/// </summary>
		private bool _jsxTransformerLoaded = false;

		/// <summary>
		/// Initializes a new instance of the <see cref="ReactEnvironment"/> class.
		/// </summary>
		/// <param name="engine">The JavaScript engine</param>
		/// <param name="config">The site-wide configuration</param>
		/// <param name="server">Server utilities</param>
		public ReactEnvironment(
			IJavascriptEngine engine, 
			IReactSiteConfiguration config,
			HttpServerUtilityBase server
		)
		{
			_engine = engine;
			_config = config;
			_server = server;
			LoadStandardScripts();
			LoadExtraScripts();
		}

		/// <summary>
		/// Loads standard React and JSXTransformer scripts.
		/// </summary>
		private void LoadStandardScripts()
		{
			_engine.Execute("var global = global || {};");
			_engine.LoadFromResource("React.Resources.react-0.9.0.js");
			_engine.Execute("var React = global.React");
		}

		/// <summary>
		/// Loads any user-supplied scripts from the configuration.
		/// </summary>
		private void LoadExtraScripts()
		{
			foreach (var file in _config.Scripts)
			{
				// TODO: Move MapPath call elsewhere?
				var fullPath = _server.MapPath(file);
				var contents = File.ReadAllText(fullPath);
				_engine.Execute(contents);
			}
		}

		/// <summary>
		/// Executes the provided JavaScript code.
		/// </summary>
		/// <param name="code">JavaScript to execute</param>
		public void Execute(string code)
		{
			_engine.Execute(code);
		}

		/// <summary>
		/// Executes the provided JavaScript code, returning a result of the specified type.
		/// </summary>
		/// <typeparam name="T">Type to return</typeparam>
		/// <param name="code">Code to execute</param>
		/// <returns>Result of the JavaScript code</returns>
		public T Execute<T>(string code)
		{
			return _engine.Execute<T>(code);
		}

		/// <summary>
		/// Creates an instance of the specified React JavaScript component.
		/// </summary>
		/// <typeparam name="T">Type of the props</typeparam>
		/// <param name="componentName">Name of the component</param>
		/// <param name="props">Props to use</param>
		/// <returns>The component</returns>
		public IReactComponent CreateComponent<T>(string componentName, T props)
		{
			_maxContainerId++;
			var containerId = string.Format(CONTAINER_ELEMENT_NAME, _maxContainerId);
			var component = new ReactComponent(this, componentName, containerId)
			{
				Props = props
			};
			_components.Add(component);
			return component;
		}

		/// <summary>
		/// Renders the JavaScript required to initialise all components client-side. This will 
		/// attach event handlers to the server-rendered HTML.
		/// </summary>
		/// <returns>JavaScript for all components</returns>
		public string GetInitJavaScript()
		{
			var fullScript = new StringBuilder();
			foreach (var component in _components)
			{
				fullScript.Append(component.RenderJavaScript());
				fullScript.AppendLine(";");
			}
			return fullScript.ToString();
		}

		/// <summary>
		/// Transforms JSX into regular JavaScript.
		/// </summary>
		/// <param name="input">JSX</param>
		/// <returns>JavaScript</returns>
		public string TransformJsx(string input)
		{
			// Lazily load the JSX transformer JavaScript
			if (!_jsxTransformerLoaded)
			{
				_engine.LoadFromResource("React.Resources.JSXTransformer.js");
				_jsxTransformerLoaded = true;
			}

			_engine.SetVariable("input", input);
			try
			{
				var output = _engine.Execute<string>(@"global.JSXTransformer.transform(input).code");
				return output;
			}
			catch (Exception ex)
			{
				throw new JsxException(ex.Message, ex);
			}
		}
	}
}
