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
	/// Base class for all ReactJS.NET exceptions
	/// </summary>
#if !NETSTANDARD1_6
	[Serializable]
#endif
	public class ReactException : Exception
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ReactException"/> class.
		/// </summary>
		public ReactException() : base() { }
		/// <summary>
		/// Initializes a new instance of the <see cref="ReactException"/> class.
		/// </summary>
		/// <param name="message">The message that describes the error.</param>
		public ReactException(string message) : base(message) { }
		/// <summary>
		/// Initializes a new instance of the <see cref="ReactException"/> class.
		/// </summary>
		/// <param name="message">The error message that explains the reason for the exception.</param>
		/// <param name="innerException">The exception that is the cause of the current exception, or a null reference (Nothing in Visual Basic) if no inner exception is specified.</param>
		public ReactException(string message, Exception innerException)
			: base(message, innerException) { }

#if !NETSTANDARD1_6
		/// <summary>
		/// Used by deserialization
		/// </summary>
		protected ReactException(SerializationInfo info, StreamingContext context) 
			: base(info, context)
		{ }
#endif
	}
}
