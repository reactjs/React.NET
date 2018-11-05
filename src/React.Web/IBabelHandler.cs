/*
 * Copyright (c) Facebook, Inc. and its affiliates.
 *
 * This source code is licensed under the MIT license found in the
 * LICENSE file in the root directory of this source tree.
 */

namespace React.Web
{
	/// <summary>
	/// ASP.NET handler that transforms JavaScript via Babel
	/// </summary>
	public interface IBabelHandler
	{
		/// <summary>
		/// Executes the handler. Outputs JavaScript to the response.
		/// </summary>
		void Execute();
	}
}
