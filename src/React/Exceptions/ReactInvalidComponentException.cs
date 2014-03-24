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
	/// Thrown when a non-existent component is rendered.
	/// </summary>
	public class ReactInvalidComponentException : ReactException
	{
		public ReactInvalidComponentException() : base() { }
		public ReactInvalidComponentException(string message) : base(message) { }
		public ReactInvalidComponentException(string message, Exception innerException)
			: base(message, innerException) { }
	}
}
