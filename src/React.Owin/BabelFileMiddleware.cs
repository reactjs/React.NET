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
using System.Threading.Tasks;

using Microsoft.Owin.StaticFiles;

namespace React.Owin
{
	/// <summary>
	/// Enables serving static JavaScript files compiled via Babel. Wraps around StaticFileMiddleware.
	/// </summary>
	public class BabelFileMiddleware
	{
		private readonly Func<IDictionary<string, object>, Task> _next;
		private readonly BabelFileOptions _options;

		static BabelFileMiddleware()
		{
			// Assume that request will ask for the "per request" instances only once. 
			Initializer.Initialize(options => options.AsMultiInstance());
		}

		/// <summary>
		/// Creates a new instance of the BabelFileMiddleware.
		/// </summary>
		/// <param name="next">The next middleware in the pipeline.</param>
		/// <param name="options">The configuration options.</param>
		public BabelFileMiddleware(Func<IDictionary<string, object>, Task> next, BabelFileOptions options)
		{
			if (next == null)
				throw new ArgumentNullException("next");

			_next = next;

			// Default values
			_options = options ?? new BabelFileOptions();
		}

		/// <summary>
		/// Processes a request to determine if it matches a known JSX file, and if so, serves it compiled to JavaScript.
		/// </summary>
		/// <param name="environment">OWIN environment dictionary which stores state information about the request, response and relevant server state.</param>
		/// <returns/>
		public async Task Invoke(IDictionary<string, object> environment)
		{
			// Create all "per request" instances
			var reactEnvironment = ReactEnvironment.Current;

			var internalStaticMiddleware = CreateFileMiddleware(reactEnvironment.Babel);
			await internalStaticMiddleware.Invoke(environment);

			// Clean up all "per request" instances
			var disposable = reactEnvironment as IDisposable;
			if (disposable != null)
				disposable.Dispose();
		}

		/// <summary>
		/// Creates the internal <see cref="StaticFileMiddleware"/> used to serve files.
		/// </summary>
		/// <param name="babel"></param>
		/// <returns></returns>
		private StaticFileMiddleware CreateFileMiddleware(IBabel babel)
		{
			return new StaticFileMiddleware(
				_next,
				new StaticFileOptions()
				{
					ContentTypeProvider = _options.StaticFileOptions.ContentTypeProvider,
					DefaultContentType = _options.StaticFileOptions.DefaultContentType,
					OnPrepareResponse = _options.StaticFileOptions.OnPrepareResponse,
					RequestPath = _options.StaticFileOptions.RequestPath,
					ServeUnknownFileTypes = _options.StaticFileOptions.ServeUnknownFileTypes,
					FileSystem = new BabelFileSystem(
						babel, 
						_options.StaticFileOptions.FileSystem, 
						_options.Extensions
					)
				});
		}
	}
}