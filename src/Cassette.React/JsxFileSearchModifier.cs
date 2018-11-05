/*
 * Copyright (c) Facebook, Inc. and its affiliates.
 *
 * This source code is licensed under the MIT license found in the
 * LICENSE file in the root directory of this source tree.
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
