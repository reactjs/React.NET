/*
 * Copyright (c) Facebook, Inc. and its affiliates.
 *
 * This source code is licensed under the MIT license found in the
 * LICENSE file in the root directory of this source tree.
 */

using System;
using System.Runtime.Serialization;

namespace React.Exceptions
{
	/// <summary>
	/// Thrown when the script pre-compilation is not available.
	/// </summary>
#if !NETSTANDARD1_6
	[Serializable]
#endif
	public class ReactScriptPrecompilationNotAvailableException : ReactException
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ReactScriptPrecompilationNotAvailableException"/> class.
		/// </summary>
		/// <param name="message">The message that describes the error.</param>
		public ReactScriptPrecompilationNotAvailableException(string message) : base(message) { }

#if !NETSTANDARD1_6
		/// <summary>
		/// Used by deserialization
		/// </summary>
		protected ReactScriptPrecompilationNotAvailableException(SerializationInfo info, StreamingContext context)
			: base(info, context) { }
#endif
	}
}
