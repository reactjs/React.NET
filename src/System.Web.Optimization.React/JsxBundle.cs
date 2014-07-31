/*
 *  Copyright (c) 2014, Facebook, Inc.
 *  All rights reserved.
 *
 *  This source code is licensed under the BSD-style license found in the
 *  LICENSE file in the root directory of this source tree. An additional grant 
 *  of patent rights can be found in the PATENTS file in the same directory.
 */

namespace System.Web.Optimization.React
{
	/// <summary>
	/// Represents a bundle that compiles JSX to JavaScript before minifying.
	/// </summary>
	public class JsxBundle : Bundle
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="JsxBundle"/> class.
		/// </summary>
		/// <param name="virtualPath">
		/// The virtual path used to reference the <see cref="T:System.Web.Optimization.Bundle" />
		/// from within a view or Web page.
		/// </param>
		public JsxBundle(string virtualPath) : base(virtualPath, GetTransforms())
		{
            base.ConcatenationToken = ";" + Environment.NewLine;
		}

        /// <summary>
        /// Applies the transformations.
        /// </summary>
        /// <returns>The bundle response.</returns>
        public override BundleResponse ApplyTransforms(BundleContext context, string bundleContent, Collections.Generic.IEnumerable<BundleFile> bundleFiles)
        {
            const string pragma = "/** @jsx React.DOM */";

            if (!bundleContent.TrimStart().StartsWith(pragma))
            {
                bundleContent = pragma + bundleContent;
            }

            return base.ApplyTransforms(context, bundleContent, bundleFiles);
        }

		/// <summary>
		/// Gets the transformations that should be used by the bundle.
		/// </summary>
		/// <returns>The transformations</returns>
		private static IBundleTransform[] GetTransforms()
		{
			return new IBundleTransform[] { new JsxTransform(), new JsMinify() };
		}
	}
}
