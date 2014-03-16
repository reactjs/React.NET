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