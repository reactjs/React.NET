/*
 *  Copyright (c) 2014-Present, Facebook, Inc.
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
	/// Represents the result of a Babel transformation along with its
	/// corresponding source map.
	/// </summary>
	[Serializable]
	public class JavaScriptWithSourceMap
	{
		/// <summary>
		/// Gets or sets the version of Babel used to perform this transformation.
		/// </summary>
		public string BabelVersion { get; set; }

		/// <summary>
		/// The transformed result
		/// </summary>
		public string Code { get; set; }

		/// <summary>
		/// The hash of the input file.
		/// </summary>
		public string Hash { get; set; }

		/// <summary>
		/// The source map for this code
		/// </summary>
		public SourceMap SourceMap { get; set; }
	}
}
