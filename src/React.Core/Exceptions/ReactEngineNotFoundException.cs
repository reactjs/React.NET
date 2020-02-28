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
	/// Thrown when no valid JavaScript engine is found.
	/// </summary>
	[Serializable]
	public class ReactEngineNotFoundException : ReactException
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ReactEngineNotFoundException"/> class.
		/// </summary>
		/// <param name="message">The message that describes the error.</param>
		public ReactEngineNotFoundException(string message) : base(message) { }

		/// <summary>
		/// Used by deserialization
		/// </summary>
		protected ReactEngineNotFoundException(SerializationInfo info, StreamingContext context)
			: base(info, context) { }
	}
}
