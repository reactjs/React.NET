/*
 *  Copyright (c) 2014-2015, Facebook, Inc.
 *  All rights reserved.
 *
 *  This source code is licensed under the BSD-style license found in the
 *  LICENSE file in the root directory of this source tree. An additional grant 
 *  of patent rights can be found in the PATENTS file in the same directory.
 */

using System.Web;

namespace React.Web
{
	/// <summary>
	/// ASP.NET handler that transforms JavaScript via Babel
	/// </summary>
	public class BabelHandler : IBabelHandler
	{
		private readonly IReactEnvironment _environment;
		private readonly IFileSystem _fileSystem;
		private readonly HttpRequestBase _request;
		private readonly HttpResponseBase _response;

		/// <summary>
		/// Initializes a new instance of the <see cref="BabelHandler"/> class.
		/// </summary>
		/// <param name="environment">The environment.</param>
		/// <param name="fileSystem">File system</param>
		/// <param name="request">HTTP request</param>
		/// <param name="response">HTTP response</param>
		public BabelHandler(
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
			if (_request.QueryString["map"] != null)
			{
				RenderSourceMap();
			}
			else
			{
				RenderFile();
			}
		}

		/// <summary>
		/// Renders the result of the tranformation via Babel.
		/// </summary>
		private void RenderFile()
		{
			var relativePath = _request.Url.LocalPath;
			var result = _environment.Babel.TransformFileWithSourceMap(relativePath);
			var sourceMapUri = GetSourceMapUri(relativePath, result.Hash);
			ConfigureCaching();
			_response.ContentType = "text/javascript";
			// The sourcemap spec says to use SourceMap, but Firefox only accepts X-SourceMap
			_response.AddHeader("SourceMap", sourceMapUri);
			_response.AddHeader("X-SourceMap", sourceMapUri);

			_response.Write(result.Code);
		}

		/// <summary>
		/// Renders the source map for this file.
		/// </summary>
		private void RenderSourceMap()
		{
			var relativePath = _request.Url.LocalPath;
			var result = _environment.Babel.TransformFileWithSourceMap(relativePath, forceGenerateSourceMap: true);
			if (result.SourceMap == null)
			{
				_response.StatusCode = 500;
				_response.StatusDescription = "Unable to generate source map";
				return;
			}
			var sourceMap = result.SourceMap.ToJson();

			ConfigureCaching();
			_response.ContentType = "application/json";
			//_response.Write(")]}\n"); // Recommended by the spec but Firefox doesn't support it
			_response.Write(sourceMap);
		}

		/// <summary>
		/// Send headers to cache the response. Only caches on the server-side for now
		/// </summary>
		private void ConfigureCaching()
		{
			_response.AddFileDependency(_fileSystem.MapPath(_request.Url.LocalPath));
			_response.Cache.SetCacheability(HttpCacheability.Server);
			_response.Cache.SetLastModifiedFromFileDependencies();
			_response.Cache.SetETagFromFileDependencies();
		}

		/// <summary>
		/// Gets the URI to the source map of the specified file
		/// </summary>
		/// <param name="relativePath">Relative path to the JavaScript file</param>
		/// <param name="hash">Hash of the file</param>
		/// <returns>URI to the file</returns>
		private static string GetSourceMapUri(string relativePath, string hash)
		{
			return string.Format("{0}?map={1}", relativePath, hash);
		}
	}
}
