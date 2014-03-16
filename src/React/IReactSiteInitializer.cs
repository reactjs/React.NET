namespace React
{
	/// <summary>
	/// Handles initialisation of React.NET for the site
	/// </summary>
	public interface IReactSiteInitializer
	{
		/// <summary>
		/// Configures React.NET for this site
		/// </summary>
		/// <param name="config">Configuration</param>
		void Configure(IReactSiteConfiguration config);
	}
}
