using System.Web;
using System.Web.Caching;

namespace React
{
	/// <summary>
	/// ASP.NET handler that transforms JSX into JavaScript.
	/// </summary>
	public class JsxHandler : IJsxHandler
	{
		private readonly IReactEnvironment _environment;
		private readonly IFileSystem _fileSystem;
		private readonly HttpRequestBase _request;
		private readonly HttpResponseBase _response;

		/// <summary>
		/// Initializes a new instance of the <see cref="JsxHandler"/> class.
		/// </summary>
		/// <param name="environment">The environment.</param>
		/// <param name="fileSystem">File system</param>
		/// <param name="request">HTTP request</param>
		/// <param name="response">HTTP response</param>
		public JsxHandler(
			IReactEnvironment environment, 
			IFileSystem fileSystem, 
			HttpRequestBase request,
			HttpResponseBase response
		)
		{
			_environment = environment;
			_fileSystem = fileSystem;
			_request = request;
			_response = response;
		}

		/// <summary>
		/// Executes the handler. Outputs JavaScript to the response.
		/// </summary>
		public void Execute()
		{
			var relativePath = _request.Url.LocalPath;
			var result = _environment.LoadJsxFile(relativePath);

			// Only cache on the server-side for now
			_response.AddCacheDependency(new CacheDependency(_fileSystem.MapPath(relativePath)));
			_response.Cache.SetCacheability(HttpCacheability.Server);

			_response.ContentType = "text/javascript";
			_response.Write(result);
		}
	}
}
