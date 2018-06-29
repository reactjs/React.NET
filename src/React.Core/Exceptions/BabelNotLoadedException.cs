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
	/// Thrown when Babel is required but has not been loaded.
	/// </summary>
#if !NETSTANDARD1_6
	[Serializable]
#endif
	public class BabelNotLoadedException : ReactException
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="BabelNotLoadedException"/> class.
		/// </summary>
		public BabelNotLoadedException() : base(GetMessage()) { }

#if !NETSTANDARD1_6
		/// <summary>
		/// Used by deserialization
		/// </summary>
		protected BabelNotLoadedException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{ }
#endif

		/// <summary>
		/// Gets a message that describes the current exception.
		/// </summary>
		private static string GetMessage()
		{
			return
				"Babel has not been loaded, so JSX transformation can not be done. Please either " +
				"transform your JavaScript files through an external tool (such as Babel, " +
				"Webpack, Browserify or Gulp) and use the \"AddScriptWithoutTransform\" method to load " +
				"them for server-side rendering, or enable the \"LoadBabel\" option in the ReactJS.NET " +
				"configuration. Refer to the ReactJS.NET documentation for more details.";
		}
	}
}
