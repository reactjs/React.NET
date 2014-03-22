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
	/// Thrown when an error occurs with parsing JSX.
	/// </summary>
	public class JsxException : ReactException
	{
		public JsxException() : base() { }
		public JsxException(string message) : base(message) { }
		public JsxException(string message, Exception innerException)
			: base(message, innerException) { }
	}
}
