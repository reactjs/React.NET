using System.Web;

namespace React
{
	/// <summary>
	/// Handles creation and execution of <see cref="IJsxHandler"/> instances.
	/// </summary>
	public class JsxHandlerFactory : IHttpHandler
	{
		/// <summary>
		/// Processes this request
		/// </summary>
		/// <param name="context">The request context</param>
		public void ProcessRequest(HttpContext context)
		{
			var handler = AssemblyRegistration.Container.Resolve<IJsxHandler>();
			handler.Execute();
		}

		/// <summary>
		/// Gets a value indicating whether another request can use the <see cref="T:System.Web.IHttpHandler" /> instance.
		/// </summary>
		/// <returns>true if the <see cref="T:System.Web.IHttpHandler" /> instance is reusable; otherwise, false.</returns>
		public bool IsReusable { get { return false; } }
	}
}
