/*
 *  Copyright (c) 2015, Facebook, Inc.
 *  All rights reserved.
 *
 *  This source code is licensed under the BSD-style license found in the
 *  LICENSE file in the root directory of this source tree. An additional grant
 *  of patent rights can be found in the PATENTS file in the same directory.
 */

using System;
using System.Runtime.Serialization;

namespace React.Exceptions
{
	/// <summary>
	/// Thrown when no valid JavaScript engine is found.
	/// </summary>
#if !NETSTANDARD1_6
	[Serializable]
#endif
	public class ReactEngineNotFoundException : ReactException
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ReactEngineNotFoundException"/> class.
		/// </summary>
		/// <param name="message">The message that describes the error.</param>
		public ReactEngineNotFoundException(string message) : base(message) { }

#if !NETSTANDARD1_6
		/// <summary>
		/// Used by deserialization
		/// </summary>
		protected ReactEngineNotFoundException(SerializationInfo info, StreamingContext context)
			: base(info, context) { }
#endif
	}
}
