/*
 * Copyright (c) Facebook, Inc. and its affiliates.
 *
 * This source code is licensed under the MIT license found in the
 * LICENSE file in the root directory of this source tree.
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
