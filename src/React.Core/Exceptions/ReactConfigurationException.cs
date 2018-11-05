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
	/// Thrown when an error occurs while reading a site configuration file.
	/// </summary>
#if !NETSTANDARD1_6
	[Serializable]
#endif
	public class ReactConfigurationException : ReactException
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ReactConfigurationException"/> class.
		/// </summary>
		/// <param name="message">The message that describes the error.</param>
		public ReactConfigurationException(string message) : base(message) { }
		/// <summary>
		/// Initializes a new instance of the <see cref="ReactConfigurationException"/> class.
		/// </summary>
		/// <param name="message">The error message that explains the reason for the exception.</param>
		/// <param name="innerException">The exception that is the cause of the current exception, or a null reference (Nothing in Visual Basic) if no inner exception is specified.</param>
		public ReactConfigurationException(string message, Exception innerException)
			: base(message, innerException) { }

#if !NETSTANDARD1_6
		/// <summary>
		/// Used by deserialization
		/// </summary>
		protected ReactConfigurationException(SerializationInfo info, StreamingContext context) 
			: base(info, context) { }
#endif
	}
}
