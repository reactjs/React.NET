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
		private readonly StaticFileMiddleware _internalStaticMiddleware;

		static JsxFileMiddleware()
		{
			Initializer.Initialize(_ => _);
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

			// Default values
			options = options ?? new JsxFileOptions();
			var extensions = (options.Extensions == null || !options.Extensions.Any()) ? new[] { ".jsx", ".js" } : options.Extensions;
			var fileOptions = options.StaticFileOptions ?? new StaticFileOptions();

			// Wrap the file system with JSX file system
			var reactEnvironment = React.AssemblyRegistration.Container.Resolve<IReactEnvironment>();
			_internalStaticMiddleware = new StaticFileMiddleware(
				next,
				new StaticFileOptions()
				{
					ContentTypeProvider = fileOptions.ContentTypeProvider,
					DefaultContentType = fileOptions.DefaultContentType,
					OnPrepareResponse = fileOptions.OnPrepareResponse,
					RequestPath = fileOptions.RequestPath,
					ServeUnknownFileTypes = fileOptions.ServeUnknownFileTypes,
					FileSystem = new JsxFileSystem(reactEnvironment.JsxTransformer, fileOptions.FileSystem, extensions)
				});
		}

		/// <summary>
		/// Processes a request to determine if it matches a known JSX file, and if so, serves it compiled to JavaScript.
		/// </summary>
		/// <param name="environment">OWIN environment dictionary which stores state information about the request, response and relevant server state.</param>
		/// <returns/>
		public Task Invoke(IDictionary<string, object> environment)
		{
			return _internalStaticMiddleware.Invoke(environment);
		}
	}
}