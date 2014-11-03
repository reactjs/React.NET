/*
 *  Copyright (c) 2014, Facebook, Inc.
 *  All rights reserved.
 *
 *  This source code is licensed under the BSD-style license found in the
 *  LICENSE file in the root directory of this source tree. An additional grant 
 *  of patent rights can be found in the PATENTS file in the same directory.
 */

using System;
using System.Runtime.Serialization;
using React.Exceptions;

namespace React.JavaScriptEngine.ClearScriptV8
{
	/// <summary>
	/// Thrown when the JavaScript engine does not support JSX transformation
	/// </summary>
	[Serializable]
	public class ClearScriptV8InitialisationException : ReactException
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ClearScriptV8InitialisationException"/> class.
		/// </summary>
		public ClearScriptV8InitialisationException(string innerMessage) :
			base(GetMessage(innerMessage)) { }

		/// <summary>
		/// Used by deserialization
		/// </summary>
		protected ClearScriptV8InitialisationException(SerializationInfo info, StreamingContext context)
			: base(info, context) { }

		/// <summary>
		/// Gets a message that describes the current exception.
		/// </summary>
		private static string GetMessage(string innerMessage)
		{
			return
				"Failed to initialise ClearScript V8. This is most likely caused by the native libraries " +
				"(ClearScriptV8-64.dll and v8-x64.dll) missing from your app's Bin directory. Please " +
				"ensure your app is referencing the JavaScriptEngineSwitcher.V8 NuGet package.\n\n" +
				"More details: " + innerMessage;
		}
	}
}
