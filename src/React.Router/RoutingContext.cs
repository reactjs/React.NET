/*
 *  Copyright (c) 2014-Present, Facebook, Inc.
 *  All rights reserved.
 *
 *  This source code is licensed under the BSD-style license found in the
 *  LICENSE file in the root directory of this source tree. An additional grant
 *  of patent rights can be found in the PATENTS file in the same directory.
 */

namespace React.Router
{
	/// <summary>
	/// Context object used during render of React Router component
	/// </summary>
	public class RoutingContext
	{
		/// <summary>
		/// HTTP Status Code.
		/// If present signifies that the given status code should be returned by server.
		/// </summary>
		public int? status { get; set; }

		/// <summary>
		/// URL to redirect to.
		/// If included this signals that React Router determined a redirect should happen.
		/// </summary>
		public string url { get; set; }
	}
}
