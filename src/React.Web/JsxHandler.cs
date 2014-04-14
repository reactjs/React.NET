/*
 *  Copyright (c) 2014, Facebook, Inc.
 *  All rights reserved.
 *
 *  This source code is licensed under the BSD-style license found in the
 *  LICENSE file in the root directory of this source tree. An additional grant 
 *  of patent rights can be found in the PATENTS file in the same directory.
 */

using System.Web;
using System.Web.Caching;

namespace React.Web
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
			var result = _environment.JsxTransformer.TransformJsxFile(relativePath);

			// Only cache on the server-side for now
			_response.AddCacheDependency(new CacheDependency(_fileSystem.MapPath(relativePath)));
			_response.Cache.SetCacheability(HttpCacheability.Server);
			_response.Cache.SetLastModifiedFromFileDependencies();
			_response.Cache.SetETagFromFileDependencies();

			_response.ContentType = "text/javascript";
			_response.Write(result);
		}
	}
}
