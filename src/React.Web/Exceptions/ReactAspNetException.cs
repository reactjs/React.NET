/*
 *  Copyright (c) 2014-2015, Facebook, Inc.
 *  All rights reserved.
 *
 *  This source code is licensed under the BSD-style license found in the
 *  LICENSE file in the root directory of this source tree. An additional grant 
 *  of patent rights can be found in the PATENTS file in the same directory.
 */

using System;
using System.Runtime.Serialization;
using React.Exceptions;

namespace React.Web.Exceptions
{
	/// <summary>
	/// Thrown when an error occurs in ReactJS.NET's ASP.NET integration.
	/// </summary>
	[Serializable]
	public class ReactAspNetException : ReactException
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ReactAspNetException"/> class.
		/// </summary>
		/// <param name="message">The message that describes the error.</param>
		public ReactAspNetException(string message) : base(message) { }

		/// <summary>
		/// Initializes a new instance of the <see cref="ReactAspNetException"/> class.
		/// </summary>
		/// <param name="message">The error message that explains the reason for the exception.</param>
		/// <param name="innerException">The exception that is the cause of the current exception, or a null reference (Nothing in Visual Basic) if no inner exception is specified.</param>
		public ReactAspNetException(string message, Exception innerException)
			: base(message, innerException) { }

		/// <summary>
		/// Used by deserialization
		/// </summary>
		protected ReactAspNetException(SerializationInfo info, StreamingContext context) 
			: base(info, context) { }
	}
}
