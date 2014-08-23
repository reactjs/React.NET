using React;

[assembly: WebActivatorEx.PreApplicationStartMethod(typeof($rootnamespace$.ReactConfig), "Configure")]

namespace $rootnamespace$
{
	public static class ReactConfig
	{
		public static void Configure()
		{
			// If you want to use fancy new ES6 features, uncomment this line:
			// See http://reactjs.net/guides/es6.html for more information.
			//ReactSiteConfiguration.Configuration.SetUseHarmony(true);

			// If you want to use server-side rendering of React components, 
			// add all the necessary JavaScript files here. This includes 
			// your components as well as all of their dependencies.
			// See http://reactjs.net/ for more information. Example:
			//ReactSiteConfiguration.Configuration
			//	.AddScript("~/Scripts/First.jsx")
			//	.AddScript("~/Scripts/Second.jsx");
		}
	}
}