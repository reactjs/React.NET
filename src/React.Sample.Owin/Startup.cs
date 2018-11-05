/*
 * Copyright (c) Facebook, Inc. and its affiliates.
 *
 * This source code is licensed under the MIT license found in the
 * LICENSE file in the root directory of this source tree.
 */

using System;
using JavaScriptEngineSwitcher.Core;
using JavaScriptEngineSwitcher.V8;
using Microsoft.Owin.FileSystems;
using Microsoft.Owin.StaticFiles;

using Owin;

using React.Owin;

namespace React.Sample.Owin
{
	internal class Startup
	{
		public void Configuration(IAppBuilder app)
		{
#if DEBUG
			app.UseErrorPage();
#endif
			
			app.Use(
				async (context, next) =>
				{
					// Log all exceptions and incoming requests
					Console.WriteLine("{0} {1} {2}", context.Request.Method, context.Request.Path, context.Request.QueryString);

					try
					{
						await next();
					}
					catch (Exception exception)
					{
						Console.WriteLine(exception.ToString());
						throw;
					}
				});

			var contentFileSystem = new PhysicalFileSystem("Content");
			app.UseBabel(new BabelFileOptions() { StaticFileOptions = new StaticFileOptions() { FileSystem = contentFileSystem }});
			app.UseFileServer(new FileServerOptions() { FileSystem = contentFileSystem });

			JsEngineSwitcher.Current.DefaultEngineName = V8JsEngine.EngineName;
			JsEngineSwitcher.Current.EngineFactories.AddV8();

			app.Use<CommentsMiddleware>();
		}
	}
}
