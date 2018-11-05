/*
 * Copyright (c) Facebook, Inc. and its affiliates.
 *
 * This source code is licensed under the MIT license found in the
 * LICENSE file in the root directory of this source tree.
 */

namespace React
{
	/// <summary>
	/// An implementation of <see cref="IFileSystem" /> that does not do any mapping of file paths.
	/// </summary>
    public class SimpleFileSystem : FileSystemBase
    {
		/// <summary>
		/// Converts a path from an application relative path (~/...) to a full filesystem path
		/// </summary>
		/// <param name="relativePath">App-relative path of the file</param>
		/// <returns>Full path of the file</returns>
	    public override string MapPath(string relativePath)
	    {
		    return relativePath;
	    }
    }
}
