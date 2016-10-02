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
	/// Thrown when the VroomJs V8 JavaScript engine fails to initialise.
	/// </summary>
#if NET40
	[Serializable]
#endif
	public class VroomJsInitialisationException : ReactException
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="VroomJsInitialisationException"/> class.
		/// </summary>
		public VroomJsInitialisationException(string innerMessage) : 
			base(GetMessage(innerMessage)) { }

#if NET40
		/// <summary>
		/// Used by deserialization
		/// </summary>
		protected VroomJsInitialisationException(SerializationInfo info, StreamingContext context)
			: base(info, context) { }
#endif

		/// <summary>
		/// Gets a message that describes the current exception.
		/// </summary>
		private static string GetMessage(string innerMessage)
		{
			return
				"Failed to initialise VroomJs. This is most likely caused by the native library " +
				"(libVroomJsNative.so) being out of date or your system lacking a compatible version of " +
				"V8. Please run Mono with the `MONO_LOG_LEVEL=debug` environment variable for " +
				"more debugging information, and refer to the ReactJS.NET Mono documentation at " +
				"http://reactjs.net/guides/mono.html for further debugging hints.\n\n " +
				"More details: " + innerMessage;
		}
	}
}
