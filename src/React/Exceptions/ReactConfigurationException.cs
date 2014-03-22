/*
 *  Copyright (c) 2014, Facebook, Inc.
 *  All rights reserved.
 *
 *  This source code is licensed under the BSD-style license found in the
 *  LICENSE file in the root directory of this source tree. An additional grant 
 *  of patent rights can be found in the PATENTS file in the same directory.
 */

using System;

namespace React.Exceptions
{
	/// <summary>
	/// Thrown when an error occurs while reading a site configuration file.
	/// </summary>
	public class ReactConfigurationException : ReactException
	{
		public ReactConfigurationException() : base() { }
		public ReactConfigurationException(string message) : base(message) { }
		public ReactConfigurationException(string message, Exception innerException)
			: base(message, innerException) { }
	}
}
