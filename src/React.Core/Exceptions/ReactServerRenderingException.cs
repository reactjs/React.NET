/*
 *  Copyright (c) 2014-Present, Facebook, Inc.
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
	/// Thrown when an error occurs during server rendering of a React component.
	/// </summary>
#if !NETSTANDARD1_6
	[Serializable]
#endif
	public class ReactServerRenderingException : ReactException
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ReactInvalidComponentException"/> class.
		/// </summary>
		/// <param name="message">The message that describes the error.</param>
		public ReactServerRenderingException(string message) : base(message) { }

		/// <summary>
		/// Initializes a new instance of the <see cref="ReactInvalidComponentException"/> class.
		/// </summary>
		/// <param name="message">The error message that explains the reason for the exception.</param>
		/// <param name="innerException">The exception that is the cause of the current exception, or a null reference (Nothing in Visual Basic) if no inner exception is specified.</param>
		public ReactServerRenderingException(string message, Exception innerException)
			: base(message, innerException) { }

#if !NETSTANDARD1_6
		/// <summary>
		/// Used by deserialization
		/// </summary>
		protected ReactServerRenderingException(SerializationInfo info, StreamingContext context)
			: base(info, context) { }
#endif
	}
}
