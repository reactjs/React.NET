/*
 *  Copyright (c) 2014-Present, Facebook, Inc.
 *  All rights reserved.
 *
 *  This source code is licensed under the BSD-style license found in the
 *  LICENSE file in the root directory of this source tree. An additional grant 
 *  of patent rights can be found in the PATENTS file in the same directory.
 */

using Cassette;
using Cassette.Scripts;
using Cassette.Stylesheets;

namespace React.Sample.Cassette
{
    /// <summary>
    /// Configures the Cassette asset bundles for the web application.
    /// </summary>
    public class CassetteBundleConfiguration : IConfiguration<BundleCollection>
    {
        public void Configure(BundleCollection bundles)
        {
            // Configure your bundles here...
            // Please read http://getcassette.net/documentation/configuration

			bundles.Add<StylesheetBundle>("main.css", "Content/Sample.css");
			bundles.Add<ScriptBundle>("main.js", "Content/Sample.jsx");
        }
    }
}
