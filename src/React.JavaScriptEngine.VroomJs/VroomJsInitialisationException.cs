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

namespace React.JavaScriptEngine.VroomJs
{
	/// <summary>
	/// Thrown when the JavaScript engine does not support JSX transformation
	/// </summary>
	[Serializable]
	public class VroomJsInitialisationException : ReactException
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="VroomJsInitialisationException"/> class.
		/// </summary>
		public VroomJsInitialisationException(string innerMessage) : 
			base(GetMessage(innerMessage)) { }

		/// <summary>
		/// Used by deserialization
		/// </summary>
		protected VroomJsInitialisationException(SerializationInfo info, StreamingContext context)
			: base(info, context) { }

		/// <summary>
		/// Gets a message that describes the current exception.
		/// </summary>
		private static string GetMessage(string innerMessage)
		{
			return
				"Failed to initialise VroomJs. This is most likely caused by the native library " +
				"(libVroomJsNative.so) being out of date or your system lacking a compatible version of " +
				"V8. Please run Mono with the `MONO_LOG_LEVEL=debug` environment variable for " +
				"more debugging information. \n\n " +
				"More details: " + innerMessage;
		}
	}
}
