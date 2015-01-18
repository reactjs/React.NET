/*
 *  Copyright (c) 2014-2015, Facebook, Inc.
 *  All rights reserved.
 *
 *  This source code is licensed under the BSD-style license found in the
 *  LICENSE file in the root directory of this source tree. An additional grant 
 *  of patent rights can be found in the PATENTS file in the same directory.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.Owin.StaticFiles;

namespace React.Owin
{
	/// <summary>
	/// Enables serving static JSX files transformed to pure JavaScript. Wraps around StaticFileMiddleware.
	/// </summary>
	public class JsxFileMiddleware
	{
		private readonly Func<IDictionary<string, object>, Task> _next;
		private readonly StaticFileOptions _fileOptions;
		private readonly IEnumerable<string> _extensions;

		static JsxFileMiddleware()
		{
			// Assume that request will ask for the "per request" instances only once. 
			Initializer.Initialize(options => options.AsMultiInstance());
		}

		/// <summary>
		/// Creates a new instance of the JsxFileMiddleware.
		/// </summary>
		/// <param name="next">The next middleware in the pipeline.</param>
		/// <param name="options">The configuration options.</param>
		public JsxFileMiddleware(Func<IDictionary<string, object>, Task> next, JsxFileOptions options)
		{
			if (next == null)
				throw new ArgumentNullException("next");

			_next = next;

			// Default values
			options = options ?? new JsxFileOptions();
			_extensions = (options.Extensions == null || !options.Extensions.Any()) ? new[] { ".jsx", ".js" } : options.Extensions;
			_fileOptions = options.StaticFileOptions ?? new StaticFileOptions();
		}

		/// <summary>
		/// Processes a request to determine if it matches a known JSX file, and if so, serves it compiled to JavaScript.
		/// </summary>
		/// <param name="environment">OWIN environment dictionary which stores state information about the request, response and relevant server state.</param>
		/// <returns/>
		public async Task Invoke(IDictionary<string, object> environment)
		{
			// Create all "per request" instances
			var reactEnvironment = React.AssemblyRegistration.Container.Resolve<IReactEnvironment>();

			var internalStaticMiddleware = CreateFileMiddleware(reactEnvironment.JsxTransformer);
			await internalStaticMiddleware.Invoke(environment);

			// Clean up all "per request" instances
			var disposable = reactEnvironment as IDisposable;
			if (disposable != null)
				disposable.Dispose();
		}

		private StaticFileMiddleware CreateFileMiddleware(IJsxTransformer jsxTransformer)
		{
			return new StaticFileMiddleware(
				_next,
				new StaticFileOptions()
				{
					ContentTypeProvider = _fileOptions.ContentTypeProvider,
					DefaultContentType = _fileOptions.DefaultContentType,
					OnPrepareResponse = _fileOptions.OnPrepareResponse,
					RequestPath = _fileOptions.RequestPath,
					ServeUnknownFileTypes = _fileOptions.ServeUnknownFileTypes,
					FileSystem = new JsxFileSystem(jsxTransformer, _fileOptions.FileSystem, _extensions)
				});
		}
	}
}