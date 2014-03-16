using System;
using System.Linq;
using React.Exceptions;
using React.TinyIoC;

namespace React
{
	/// <summary>
	/// Handles loading of React.NET configuration from the site.
	/// </summary>
	internal static class ConfigurationFactory
	{
		/// <summary>
		/// Tries to find a configuration class via Reflection. The site should have exactly one
		/// implementation of <c>IReactSiteInitializer</c>, otherwise an exception will be thrown.
		/// </summary>
		/// <param name="container">IoC container</param>
		/// <param name="overloads"></param>
		/// <returns>React site configuration</returns>
		/// <exception cref="ReactConfigurationException">
		/// Thrown if no valid configuration can be found
		/// </exception>
		public static IReactSiteConfiguration GetConfiguration(
			TinyIoCContainer container, 
			NamedParameterOverloads overloads
		)
		{
			// Find all classes that implement IReactSiteInitializer
			var initializerTypes = AppDomain.CurrentDomain.GetAssemblies()
				.SelectMany(assembly => assembly.GetTypes())
				.Where(type => type.GetInterfaces().Contains(typeof(IReactSiteInitializer)))
				.ToList();

			if (initializerTypes.Count == 0)
			{
				throw new ReactConfigurationException(
					"Could not find React site configuration. Please create a class that inherits " +
					"from IReactSiteInitializer to initialize React for this site."
				);
			}
			if (initializerTypes.Count > 1)
			{
				throw new ReactConfigurationException(
					"Found multiple implementations of IReactSiteInitializer. Unable to continue. " +
					"Please ensure your site only has one."
				);
			}

			// Instantiate IReactSiteInitializer to get configuration.
			var initializer = (IReactSiteInitializer) Activator.CreateInstance(initializerTypes[0]);
			var config = new ReactSiteConfiguration();
			initializer.Configure(config);
			return config;
		}
	}
}
