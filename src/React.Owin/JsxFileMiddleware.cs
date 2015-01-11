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

            // Register interface implementations required by React. By default it sets React to work with physical file system.
            var container = AssemblyRegistration.Container;
            IFileSystem tempFileSystem;
            if (!container.TryResolve(out tempFileSystem))
                AssemblyRegistration.Container.Register<IFileSystem, EntryAssemblyFileSystem>();

            ICache tempCache;
            if (!container.TryResolve(out tempCache))
                AssemblyRegistration.Container.Register<ICache, MemoryFileCache>();
        }

        public JsxFileMiddleware(Func<IDictionary<string, object>, Task> next, JsxFileOptions options)
        {
            if (next == null)
                throw new ArgumentNullException("next");

            // Default values
            options = options ?? new JsxFileOptions();
            var extenstions = (options.Extensions == null || !options.Extensions.Any()) ? new[] { ".jsx" } : options.Extensions;
            var fileOptions = options.StaticFileOptions ?? new StaticFileOptions();

            // Wrap the file system with JSX file system
            var reactEnvironment = AssemblyRegistration.Container.Resolve<IReactEnvironment>();
            _internalStaticMiddleware = new StaticFileMiddleware(
                next,
                new StaticFileOptions()
                {
                    ContentTypeProvider = fileOptions.ContentTypeProvider,
                    DefaultContentType = fileOptions.DefaultContentType,
                    OnPrepareResponse = fileOptions.OnPrepareResponse,
                    RequestPath = fileOptions.RequestPath,
                    ServeUnknownFileTypes = fileOptions.ServeUnknownFileTypes,
                    FileSystem = new JsxFileSystem(reactEnvironment.JsxTransformer, fileOptions.FileSystem, extenstions)
                });
        }

        public Task Invoke(IDictionary<string, object> environment)
        {
            return _internalStaticMiddleware.Invoke(environment);
        }
    }
}