/*
 *  Copyright (c) 2014, Facebook, Inc.
 *  All rights reserved.
 *
 *  This source code is licensed under the BSD-style license found in the
 *  LICENSE file in the root directory of this source tree. An additional grant 
 *  of patent rights can be found in the PATENTS file in the same directory.
 */

using JavaScriptEngineSwitcher.Core;
using JavaScriptEngineSwitcher.Jint;

namespace React
{
	/// <summary>
	/// Extension methods for <see cref="IJsEngine" />.
	/// </summary>
	public static class JavaScriptEngineExtensions
	{
		/// <summary>
		/// Determines if this JavaScript engine supports the JSX transformer.
		/// </summary>
		/// <param name="jsEngine">JavaScript engine</param>
		/// <returns><c>true</c> if JSXTransformer is supported</returns>
		public static bool SupportsJsxTransformer(this IJsEngine jsEngine)
		{
			// Jint overflows the stack if you attempt to run the JSX Transformer :(
			return !(jsEngine is JintJsEngine);
		}
	}
}
