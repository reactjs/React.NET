/*
 *  Copyright (c) 2015, Facebook, Inc.
 *  All rights reserved.
 *
 *  This source code is licensed under the BSD-style license found in the
 *  LICENSE file in the root directory of this source tree. An additional grant 
 *  of patent rights can be found in the PATENTS file in the same directory.
 */

[assembly: WebActivatorEx.PreApplicationStartMethod(typeof(React.Sample.Webpack.ReactConfig), "Configure")]

namespace React.Sample.Webpack
{
	public static class ReactConfig
	{
		public static void Configure()
		{
			ReactSiteConfiguration.Configuration
				.AddScript("~/build/server.bundle.js");
		}
	}
}