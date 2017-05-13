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

namespace React.Router
{
	/// <summary>
	/// React Router Exception
	/// </summary>
#if NET451
	[Serializable]
#endif
	public class ReactRouterException : Exception
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ReactRouterException "/> class.
		/// </summary>
		public ReactRouterException() : base() { }
		/// <summary>
		/// Initializes a new instance of the <see cref="ReactRouterException "/> class.
		/// </summary>
		/// <param name="message">The message that describes the error.</param>
		public ReactRouterException(string message) : base(message) { }
		/// <summary>
		/// Initializes a new instance of the <see cref="ReactRouterException "/> class.
		/// </summary>
		/// <param name="message">The error message that explains the reason for the exception.</param>
		/// <param name="innerException">The exception that is the cause of the current exception, or a null reference (Nothing in Visual Basic) if no inner exception is specified.</param>
		public ReactRouterException(string message, Exception innerException)
			: base(message, innerException) { }


#if NET451
		/// <summary>
		/// Used by deserialization
		/// </summary>
		protected ReactRouterException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{ }
#endif
	}
}
