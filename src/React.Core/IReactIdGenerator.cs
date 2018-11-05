/*
 * Copyright (c) Facebook, Inc. and its affiliates.
 *
 * This source code is licensed under the MIT license found in the
 * LICENSE file in the root directory of this source tree.
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
