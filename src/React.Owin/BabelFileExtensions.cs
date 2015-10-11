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