/*
 *  Copyright (c) 2014, Facebook, Inc.
 *  All rights reserved.
 *
 *  This source code is licensed under the BSD-style license found in the
 *  LICENSE file in the root directory of this source tree. An additional grant 
 *  of patent rights can be found in the PATENTS file in the same directory.
 */

using System.Web.Optimization;
using System.Web.Optimization.React;

namespace React.Sample.Mvc4
{
	public static class BundleConfig
	{
		// For more information on Bundling, visit http://go.microsoft.com/fwlink/?LinkId=254725
		public static void RegisterBundles(BundleCollection bundles)
		{
			bundles.Add(new JsxBundle("~/bundles/main").Include(
				// Add your JSX files here
				"~/Content/Sample.jsx"	
			));

			// Force minification/combination even in debug mode
			BundleTable.EnableOptimizations = true;
		}
	}
}