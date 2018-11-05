/*
 * Copyright (c) Facebook, Inc. and its affiliates.
 *
 * This source code is licensed under the MIT license found in the
 * LICENSE file in the root directory of this source tree.
 */

using Owin;

namespace React.Owin
{
	/// <summary>
	/// Extensions for BabelFileMiddleware.
	/// </summary>
	public static class BabelFileExtensions
	{
		/// <summary>
		/// Enables serving JavaScript files compiled via Babel.
		/// </summary>
		public static IAppBuilder UseBabel(this IAppBuilder builder, BabelFileOptions options = null)
		{
			return builder.Use<BabelFileMiddleware>(options ?? new BabelFileOptions());
		}
	}
}
