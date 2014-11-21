namespace React
{
	/// <summary>
	/// Represents a React JavaScript component.
	/// </summary>
	public class ReactClientComponent : ReactComponent
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ReactComponent"/> class.
		/// </summary>
		/// <param name="environment">The environment.</param>
		/// <param name="configuration">Site-wide configuration.</param>
		/// <param name="componentName">Name of the component.</param>
		/// <param name="containerId">The ID of the container DIV for this component</param>
		public ReactClientComponent(IReactEnvironment environment, IReactSiteConfiguration configuration, string componentName, string containerId)
			: base(environment, configuration, componentName, containerId)
		{
		}

		/// <summary>
		/// We can't verify this without running the scripts so we will assume that it does
		/// </summary>
		protected override void EnsureComponentExists()
		{
			//Intentionally left empty. We can't verify without running scripts.
		}

		/// <summary>
		/// Renders the HTML for this component. This will just render an empty container ready for react 
		/// to initialize on the client.
		/// </summary>
		/// <returns>HTML</returns>
		public override string RenderHtml()
		{
			return string.Format(
				"<{2} id=\"{0}\">{1}</{2}>",
				ContainerId,
				string.Empty,
				ContainerTag
				);
		}
	}
}