/*
 * Copyright (c) Facebook, Inc. and its affiliates.
 *
 * This source code is licensed under the MIT license found in the
 * LICENSE file in the root directory of this source tree.
 */

using React;
using System.Linq;

namespace System.Web.Optimization.React
{
	/// <summary>
	/// Transforms JavaScript via Babel. Should be included before any minification 
	/// transformations.
	/// </summary>
	public class BabelTransform : IBundleTransform
	{
		/// <summary>
		/// Transforms the content in the <see cref="T:System.Web.Optimization.BundleResponse" /> object.
		/// </summary>
		/// <param name="context">The bundle context.</param>
		/// <param name="response">The bundle response.</param>
		public void Process(BundleContext context, BundleResponse response)
		{
			var environment = ReactEnvironment.Current;
			response.Content = environment.Babel.Transform(
				response.Content,
				response.Files.Any(x => x.IncludedVirtualPath.Contains("tsx")) ? "components.tsx" : "components.jsx"
			);
		}
	}
}
