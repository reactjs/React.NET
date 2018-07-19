/*
 *  Copyright (c) 2016-Present, Facebook, Inc.
 *  All rights reserved.
 *
 *  This source code is licensed under the BSD-style license found in the
 *  LICENSE file in the root directory of this source tree. An additional grant 
 *  of patent rights can be found in the PATENTS file in the same directory.
 */

namespace React
{
	/// <summary>
	/// Fast component ID generator
	/// </summary>
	public interface IReactIdGenerator
	{
		/// <summary>
		/// Returns a short react identifier starts with "react_".
		/// </summary>
		/// <returns></returns>
		string Generate();
	}
}
