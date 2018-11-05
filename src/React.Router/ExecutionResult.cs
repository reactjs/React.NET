/*
 * Copyright (c) Facebook, Inc. and its affiliates.
 *
 * This source code is licensed under the MIT license found in the
 * LICENSE file in the root directory of this source tree.
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
