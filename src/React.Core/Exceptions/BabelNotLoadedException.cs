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
	/// Thrown when Babel is required but has not been loaded.
	/// </summary>
	[Serializable]
	public class BabelNotLoadedException : ReactException
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="BabelNotLoadedException"/> class.
		/// </summary>
		public BabelNotLoadedException() : base(GetMessage()) { }

		/// <summary>
		/// Used by deserialization
		/// </summary>
		protected BabelNotLoadedException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{ }

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
