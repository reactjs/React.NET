/*
 *  Copyright (c) 2014, Facebook, Inc.
 *  All rights reserved.
 *
 *  This source code is licensed under the BSD-style license found in the
 *  LICENSE file in the root directory of this source tree. An additional grant 
 *  of patent rights can be found in the PATENTS file in the same directory.
 */

namespace React
{
	/// <summary>
	/// Handles compiling JSX to JavaScript.
	/// </summary>
	public interface IJsxTransformer
	{
		/// <summary>
		/// Transforms a JSX file. Results of the JSX to JavaScript transformation are cached.
		/// </summary>
		/// <param name="filename">Name of the file to load</param>
		/// <returns>JavaScript</returns>
		string TransformJsxFile(string filename);

		/// <summary>
		/// Transforms JSX into regular JavaScript. The result is not cached. Use 
		/// <see cref="TransformJsxFile"/> if loading from a file since this will cache the result.
		/// </summary>
		/// <param name="input">JSX</param>
		/// <returns>JavaScript</returns>
		string TransformJsx(string input);

		/// <summary>
		/// Transforms a JSX file to JavaScript, and saves the result into a ".generated.js" file 
		/// alongside the original file.
		/// </summary>
		/// <param name="filename">Name of the file to load</param>
		/// <returns>File contents</returns>
		string TransformAndSaveJsxFile(string filename);

		/// <summary>
		/// Returns the path the specified JSX file's compilation will be cached to
		/// </summary>
		/// <param name="path">Path of the JSX file</param>
		/// <returns>Output path of the compiled file</returns>
		string GetJsxOutputPath(string path);
	}
}