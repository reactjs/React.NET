/*
 *  Copyright (c) 2014, Facebook, Inc.
 *  All rights reserved.
 *
 *  This source code is licensed under the BSD-style license found in the
 *  LICENSE file in the root directory of this source tree. An additional grant 
 *  of patent rights can be found in the PATENTS file in the same directory.
 */

using Cassette.Scripts;

namespace Cassette.React
{
	/// <summary>
	/// Adds *.jsx to the file search path for script bundles.
	/// </summary>
	public class JsxFileSearchModifier : IFileSearchModifier<ScriptBundle>
	{
		/// <summary>
		/// Modifies the specified file search.
		/// </summary>
		/// <param name="fileSearch">The file search.</param>
		public void Modify(FileSearch fileSearch)
		{
			fileSearch.Pattern += ";*.jsx";
		}
	}
}
