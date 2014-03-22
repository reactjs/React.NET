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
	/// Base class for all React.NET exceptions
	/// </summary>
	public class ReactException : Exception
	{
		public ReactException() : base() { }
		public ReactException(string message) : base(message) { }
		public ReactException(string message, Exception innerException)
			: base(message, innerException) { }
	}
}
