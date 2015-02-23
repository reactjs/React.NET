/*
 *  Copyright (c) 2015, Facebook, Inc.
 *  All rights reserved.
 *
 *  This source code is licensed under the BSD-style license found in the
 *  LICENSE file in the root directory of this source tree. An additional grant 
 *  of patent rights can be found in the PATENTS file in the same directory.
 */

using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Hosting;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.StaticFiles;
using Microsoft.Framework.Logging;

namespace React.AspNet5
{
	/// <summary>
	/// Enables serving static JSX files transformed to pure JavaScript. Wraps around StaticFileMiddleware.
	/// </summary>
	public class JsxFileMiddleware
	{
		private readonly RequestDelegate _next;
		private readonly IHostingEnvironment _hostingEnv;
		private readonly ILoggerFactory _loggerFactory;
		private readonly JsxFileOptions _options;

		/// <summary>
		/// Creates a new instance of the JsxFileMiddleware.
		/// </summary>
		/// <param name="next">The next middleware in the pipeline.</param>
		/// <param name="options">The configuration options.</param>
		/// <param name="hostingEnv">The hosting environment.</param>
		/// <param name="loggerFactory">An <see cref="ILoggerFactory"/> instance used to create loggers.</param>
		public JsxFileMiddleware(RequestDelegate next, JsxFileOptions options, IHostingEnvironment hostingEnv, ILoggerFactory loggerFactory)
		{
			if (next == null)
				throw new ArgumentNullException("next");

			_next = next;
			_hostingEnv = hostingEnv;
			_loggerFactory = loggerFactory;

			// Default values
			_options = options ?? new JsxFileOptions();
		}

		/// <summary>
		/// Processes a request to determine if it matches a known JSX file, and if so, serves it compiled to JavaScript.
		/// </summary>
		/// <param name="context">ASP.NET HTTP context</param>
		public async Task Invoke(HttpContext context)
		{
			if (!context.Request.Path.HasValue || !_options.Extensions.Any(context.Request.Path.Value.EndsWith))
			{
				// Not a request for a JSX file, so just pass through to the next middleware
				await _next(context);
				return;
			}

			var reactEnvironment = React.AssemblyRegistration.Container.Resolve<IReactEnvironment>();
			var internalStaticMiddleware = CreateFileMiddleware(reactEnvironment.JsxTransformer);
			await internalStaticMiddleware.Invoke(context);
		}

		/// <summary>
		/// Creates the internal <see cref="StaticFileMiddleware"/> used to serve JSX files.
		/// </summary>
		/// <param name="jsxTransformer"></param>
		/// <returns></returns>
		private StaticFileMiddleware CreateFileMiddleware(IJsxTransformer jsxTransformer)
		{
			return new StaticFileMiddleware(
				_next,
				_hostingEnv,
				new StaticFileOptions
				{
					ContentTypeProvider = _options.StaticFileOptions.ContentTypeProvider,
					DefaultContentType = _options.StaticFileOptions.DefaultContentType,
					OnPrepareResponse = _options.StaticFileOptions.OnPrepareResponse,
					RequestPath = _options.StaticFileOptions.RequestPath,
					ServeUnknownFileTypes = _options.StaticFileOptions.ServeUnknownFileTypes,
					FileSystem = new JsxFileSystem(
						jsxTransformer, 
						_options.StaticFileOptions.FileSystem ?? _hostingEnv.WebRootFileSystem,
						_options.Extensions
					)
				},
				_loggerFactory
			);
		}
	}
}
 