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
	/// ASP.NET handler that transforms JSX into JavaScript.
	/// </summary>
	public interface IJsxHandler
	{
		/// <summary>
		/// Executes the handler. Outputs JavaScript to the response.
		/// </summary>
		void Execute();
	}
}