/*
 *  Copyright (c) 2016-Present, Facebook, Inc.
 *  All rights reserved.
 *
 *  This source code is licensed under the BSD-style license found in the
 *  LICENSE file in the root directory of this source tree. An additional grant 
 *  of patent rights can be found in the PATENTS file in the same directory.
 */

using System;

namespace React
{
	/// <summary>
	/// Extension methods relating to GUIDs.
	/// </summary>
	public static class GuidExtensions
	{
		/// <summary>
		/// Returns a short identifier for this GUID.
		/// </summary>
		/// <param name="guid">The unique identifier.</param>
		/// <returns>A short version of the unique identifier</returns>
		public static string ToShortGuid(this Guid guid)
		{
			return Convert.ToBase64String(guid.ToByteArray())
				.Replace("/", string.Empty)
				.Replace("+", string.Empty)
				.TrimEnd('=');
		}
	}
}
