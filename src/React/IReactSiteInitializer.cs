/*
 *  Copyright (c) 2014, Facebook, Inc.
 *  All rights reserved.
 *
 *  This source code is licensed under the BSD-style license found in the
 *  LICENSE file in the root directory of this source tree. An additional grant 
 *  of patent rights can be found in the PATENTS file in the same directory.
 */

namespace React
{
	/// <summary>
	/// Handles initialisation of React.NET for the site
	/// </summary>
	public interface IReactSiteInitializer
	{
		/// <summary>
		/// Configures React.NET for this site
		/// </summary>
		/// <param name="config">Configuration</param>
		void Configure(IReactSiteConfiguration config);
	}
}
