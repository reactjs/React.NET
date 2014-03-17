using System.Diagnostics;
using System.Web;
using React.TinyIoC;

namespace React
{
	/// <summary>
	/// Handles registration of core React.NET components.
	/// </summary>
	public class AssemblyRegistration : IAssemblyRegistration
	{
		/// <summary>
		/// Gets the IoC container. Try to avoid using this and always use constructor injection.
		/// This should only be used at the root level of an object heirarchy.
		/// </summary>
		public static TinyIoCContainer Container
		{
			get { return TinyIoCContainer.Current; }
		}

		/// <summary>
		/// Registers standard components in the React IoC container
		/// </summary>
		/// <param name="container">Container to register components in</param>
		public void Register(TinyIoCContainer container)
		{
			// One instance shared for the whole app
			container.Register<IReactSiteConfiguration>(ConfigurationFactory.GetConfiguration);

			// Unique per request
			container.Register<IReactEnvironment, ReactEnvironment>().AsPerRequestSingleton();
			container.Register<HttpContextBase>((c, o) => new HttpContextWrapper(HttpContext.Current));
			container.Register<HttpServerUtilityBase>((c, o) => c.Resolve<HttpContextBase>().Server);

			RegisterJavascriptEngine(container);
		}

		/// <summary>
		/// Registers the most appropriate JavaScript engine for the current environment. If the 
		/// MSIE engine is supported, use it, otherwise use Jint.
		/// </summary>
		/// <param name="container">IoC container</param>
		private void RegisterJavascriptEngine(TinyIoCContainer container)
		{
			// Try to use MSIE JavaScript engine, but fall back to Jint if MSIE is not supported
			// by the current operating system environment.
			if (MsieJavascriptEngine.IsSupported())
			{
				Trace.WriteLine("Using MsieJavascriptEngine");
				container.Register<IJavascriptEngine, MsieJavascriptEngine>().AsPerRequestSingleton();
			}
			else
			{
				Trace.WriteLine("No MSIE support; using JintJavascriptEngine");
				container.Register<IJavascriptEngine, JintJavascriptEngine>().AsPerRequestSingleton();
			}
		}
	}
}
