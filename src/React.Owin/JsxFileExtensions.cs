/*
 *  Copyright (c) 2014-2015, Facebook, Inc.
 *  All rights reserved.
 *
 *  This source code is licensed under the BSD-style license found in the
 *  LICENSE file in the root directory of this source tree. An additional grant 
 *  of patent rights can be found in the PATENTS file in the same directory.
 */

using Owin;

namespace React.Owin
{
    /// <summary>
    /// Extensions for JsxFileMiddleware.
    /// </summary>
    public static class JsxFileExtensions
    {
        /// <summary>
        /// Enables serving static JSX file, compiled to JavaScript, for the current request path from the current directory.
        /// </summary> 
        public static IAppBuilder UseJsxFiles(this IAppBuilder builder)
        {
            return builder.UseJsxFiles(new JsxFileOptions());
        }

        /// <summary>
        /// Enables serving static JSX file, compiled to JavaScript with the given options.
        /// </summary>
        public static IAppBuilder UseJsxFiles(this IAppBuilder builder, JsxFileOptions options)
        {
            return builder.Use<JsxFileMiddleware>(options);
        }
    }
}