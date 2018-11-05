/*
 * Copyright (c) Facebook, Inc. and its affiliates.
 *
 * This source code is licensed under the MIT license found in the
 * LICENSE file in the root directory of this source tree.
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
			bundles.Add(new BabelBundle("~/bundles/main").Include(
				// Add your JSX files here
				"~/Content/Sample.jsx"	
			));

			// Force minification/combination even in debug mode
			BundleTable.EnableOptimizations = true;
		}
	}
}
