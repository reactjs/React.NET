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
	/// Contains the context object used during execution in addition to 
	/// the string result of rendering the React Router component.
	/// </summary>
	public class ExecutionResult
	{
		/// <summary>
		/// String result of ReactDOMServer render of provided component.
		/// </summary>
		public string RenderResult { get; set; }

		/// <summary>
		/// Context object used during JS engine execution.
		/// </summary>
		public RoutingContext Context { get; set; }
	}
}
