/*
 * Copyright (c) Facebook, Inc. and its affiliates.
 *
 * This source code is licensed under the MIT license found in the
 * LICENSE file in the root directory of this source tree.
 */

using System;

namespace React
{
	/// <summary>
	/// Represents the result of a Babel transformation along with its
	/// corresponding source map.
	/// </summary>
#if !NETSTANDARD1_6
	[Serializable]
#endif
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
