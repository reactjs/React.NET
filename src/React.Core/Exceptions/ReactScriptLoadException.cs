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
	/// Thrown when an error is encountered while loading a JavaScript file.
	/// </summary>
	[Serializable]
	public class ReactScriptLoadException : ReactException
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ReactScriptLoadException"/> class.
		/// </summary>
		/// <param name="message">The message that describes the error.</param>
		public ReactScriptLoadException(string message) : base(message) { }

		/// <summary>
		/// Initializes a new instance of the <see cref="ReactScriptLoadException"/> class.
		/// </summary>
		/// <param name="message">The error message that explains the reason for the exception.</param>
		/// <param name="innerException">The exception that is the cause of the current exception, or a null reference (Nothing in Visual Basic) if no inner exception is specified.</param>
		public ReactScriptLoadException(string message, Exception innerException)
			: base(message, innerException) { }

		/// <summary>
		/// Used by deserialization
		/// </summary>
		protected ReactScriptLoadException(SerializationInfo info, StreamingContext context)
			: base(info, context) { }
	}
}
