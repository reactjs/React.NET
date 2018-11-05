/*
 * Copyright (c) Facebook, Inc. and its affiliates.
 *
 * This source code is licensed under the MIT license found in the
 * LICENSE file in the root directory of this source tree.
 */

namespace System.Web.Optimization.React
{
	/// <summary>
	/// Represents a bundle that compiles JavaScript via Babel before minifying.
	/// </summary>
	public class BabelBundle : Bundle
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="BabelBundle"/> class.
		/// </summary>
		/// <param name="virtualPath">
		/// The virtual path used to reference the <see cref="T:System.Web.Optimization.Bundle" />
		/// from within a view or Web page.
		/// </param>
		public BabelBundle(string virtualPath) : base(virtualPath, GetTransforms())
		{
			base.ConcatenationToken = ";" + Environment.NewLine;
		}

		/// <summary>
		/// Gets the transformations that should be used by the bundle.
		/// </summary>
		/// <returns>The transformations</returns>
		private static IBundleTransform[] GetTransforms()
		{
			return new IBundleTransform[] { new BabelTransform(), new JsMinify() };
		}
	}
}
