namespace React
{
	/// <summary>
	/// Request-specific client only ReactJS.NET environment. This is unique to the individual request and is 
	/// not shared.
	/// </summary>
	public interface IReactClientEnvironment
	{
		/// <summary>
		/// Creates an instance of the specified React JavaScript component.
		/// </summary>
		/// <typeparam name="T">Type of the props</typeparam>
		/// <param name="componentName">Name of the component</param>
		/// <param name="props">Props to use</param>
		/// <returns>The component</returns>
		IReactComponent CreateComponent<T>(string componentName, T props);

		/// <summary>
		/// Renders the JavaScript required to initialise all components client-side. This will 
		/// attach event handlers to the server-rendered HTML.
		/// </summary>
		/// <returns>JavaScript for all components</returns>
		string GetInitJavaScript();
	}
}