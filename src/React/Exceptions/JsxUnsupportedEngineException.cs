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
using System.Text;

namespace React.Exceptions
{
	/// <summary>
	/// Thrown when the JavaScript engine does not support JSX transformation
	/// </summary>
	[Serializable]
	public class JsxUnsupportedEngineException : ReactException
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="JsxUnsupportedEngineException"/> class.
		/// </summary>
		public JsxUnsupportedEngineException() : base(GetMessage()) { }

		/// <summary>
		/// Used by deserialization
		/// </summary>
		protected JsxUnsupportedEngineException(SerializationInfo info, StreamingContext context)
			: base(info, context) { }

		/// <summary>
		/// Gets a message that describes the current exception.
		/// </summary>
		private static string GetMessage()
		{
			return 
				"The current JavaScript engine does not support compilation of JSX files. If " +
				"you are on Windows, try upgrading your version of Internet Explorer to 9 or " +
				"above to use the updated engine. \n\nJSX Transformation is currently " +
				"unsupported on Mono. If you are using Mono, it is suggested to precompile all " +
				"JSX files on Windows before deployment. Refer to the ReactJS.NET documentation " +
				"for more details.";
		}
	}
}
