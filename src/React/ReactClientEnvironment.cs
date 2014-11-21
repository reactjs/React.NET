using System.Collections.Generic;
using System.Text;

namespace React
{
	/// <summary>
	/// Request-specific client only ReactJS.NET environment. This is unique to the individual request and is 
	/// not shared.
	/// </summary>
	public class ReactClientEnvironment : IReactClientEnvironment
	{
		private readonly string CONTAINER_ELEMENT_NAME = "ReactClient{0}";
		private int _maxContainerId = 0;
		private readonly List<IReactComponent> _components = new List<IReactComponent>();
		private readonly IReactEnvironment _reactEnvironment;
		private readonly IReactSiteConfiguration _config;

		/// <summary>
		/// Initializes a new instance of the <see cref="ReactClientEnvironment"/> class.
		/// </summary>
		/// <param name="reactEnvironment">The React environment</param>
		/// <param name="config">The site-wide configuration</param>
		public ReactClientEnvironment(IReactEnvironment reactEnvironment, IReactSiteConfiguration config)
		{
			_reactEnvironment = reactEnvironment;
			_config = config;
		}

		/// <summary>
		/// Creates an instance of the specified React JavaScript component.
		/// </summary>
		/// <typeparam name="T">Type of the props</typeparam>
		/// <param name="componentName">Name of the component</param>
		/// <param name="props">Props to use</param>
		/// <returns>The component</returns>
		public virtual IReactComponent CreateComponent<T>(string componentName, T props)
		{
			_maxContainerId++;
			var containerId = string.Format(CONTAINER_ELEMENT_NAME, _maxContainerId);
			var component = new ReactClientComponent(_reactEnvironment, _config, componentName, containerId)
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
		public virtual string GetInitJavaScript()
		{
			var fullScript = new StringBuilder();
			foreach (var component in _components)
			{
				fullScript.Append(component.RenderJavaScript());
				fullScript.AppendLine(";");
			}
			return fullScript.ToString();
		}
	}
}