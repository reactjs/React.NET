namespace React.Router
{
	/// <summary>
	/// <see cref="ReactEnvironment"/> extension for rendering a React Router Component with context
	/// </summary>
	public static class ReactEnvironmentExtensions
    {
		/// <summary>
		/// Create a React Router Component with context and add it to the list of components to render client side,
		/// if applicable.
		/// </summary>
		/// <typeparam name="T">Type of the props</typeparam>
		/// <param name="env">React Environment</param>
		/// <param name="componentName">Name of the component</param>
		/// <param name="props">Props to use</param>
		/// <param name="containerId">ID to use for the container HTML tag. Defaults to an auto-generated ID</param>
		/// <param name="clientOnly">True if server-side rendering will be bypassed. Defaults to false.</param>
		/// <returns></returns>
		public static ReactRouterComponent CreateRouterComponent<T>(
            this IReactEnvironment env,
            string componentName,
            T props,
			string containerId = null,
			bool clientOnly = false
		)
        {
			var config = React.AssemblyRegistration.Container.Resolve<IReactSiteConfiguration>();

			var component = new ReactRouterComponent(env, config, componentName, containerId)
			{
				Props = props,
			};

			var reactEnvironment = env as ReactEnvironment;

			if (reactEnvironment != null)
			{
				reactEnvironment.CreateComponent(component, clientOnly);
				return component;
			}
			else
			{
				throw new ReactRouterException("Only the default ReactEnvironment is intended to be used with React.Router");
			}
		}
    }
}
