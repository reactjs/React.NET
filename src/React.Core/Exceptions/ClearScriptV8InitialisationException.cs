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
	/// Thrown when the ClearScript V8 JavaScript engine fails to initialise
	/// </summary>
#if NET40
	[Serializable]
#endif
	public class ClearScriptV8InitialisationException : ReactException
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ClearScriptV8InitialisationException"/> class.
		/// </summary>
		public ClearScriptV8InitialisationException(Exception innerException) :
			base(GetMessage(innerException), innerException) { }

#if NET40
		/// <summary>
		/// Used by deserialization
		/// </summary>
		protected ClearScriptV8InitialisationException(SerializationInfo info, StreamingContext context)
			: base(info, context) { }
#endif

		/// <summary>
		/// Gets a message that describes the current exception.
		/// </summary>
		private static string GetMessage(Exception innerException)
		{
			return
				"Failed to initialise ClearScript V8. This is most likely caused by the native libraries " +
				"(ClearScriptV8-64.dll and v8-x64.dll) missing from your app's Bin directory, or the " +
				"Visual C++ runtime not being installed. Please ensure your app is referencing the " +
				"JavaScriptEngineSwitcher.V8 NuGet package, and refer to the ReactJS.NET site for more" +
				"debugging tips.\n\n" +
				"More details: " + innerException.Message;
		}
	}
}
