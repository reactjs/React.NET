using React.TinyIoC;

namespace React
{
	/// <summary>
	/// IoC component registration. Used to register components in the React.NET IoC container. 
	/// Every React.NET assembly should have an instance of IComponentRegistration.
	/// </summary>
	public interface IAssemblyRegistration
	{
		/// <summary>
		/// Registers components in the React.NET IoC container
		/// </summary>
		/// <param name="container">Container to register components in</param>
		void Register(TinyIoCContainer container);
	}
}
