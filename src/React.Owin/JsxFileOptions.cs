/*
 *  Copyright (c) 2014-2015, Facebook, Inc.
 *  All rights reserved.
 *
 *  This source code is licensed under the BSD-style license found in the
 *  LICENSE file in the root directory of this source tree. An additional grant 
 *  of patent rights can be found in the PATENTS file in the same directory.
 */

using System.Collections.Generic;

using Microsoft.Owin.StaticFiles;

namespace React.Owin
{
    /// <summary>
    /// Options for serving JSX files.
    /// </summary>
    public class JsxFileOptions
    {
        /// <summary>
        /// Collection of extensions that will be treated as JSX files. Defaults to ".jsx".
        /// </summary>
        public IEnumerable<string> Extensions { get; set; }

        /// <summary>
        /// Options for static file middleware used to server JSX files.
        /// </summary>
        public StaticFileOptions StaticFileOptions { get; set; }
    }
}