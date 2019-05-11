/*
 * Copyright (c) Facebook, Inc. and its affiliates.
 *
 * This source code is licensed under the MIT license found in the
 * LICENSE file in the root directory of this source tree.
 */

namespace React
{
	/// <summary>
	/// Handles compiling JavaScript files via Babel (http://babeljs.io/).
	/// </summary>
	public interface IBabel
	{
		/// <summary>
		/// Transforms a JavaScript file. Results of the transformation are cached.
		/// </summary>
		/// <param name="filename">Name of the file to load</param>
		/// <returns>JavaScript</returns>
		string TransformFile(string filename);

		/// <summary>
		/// Transforms a JavaScript file via Babel and also returns a source map to map the
		/// compiled source to the original version. Results of the transformation are cached.
		/// </summary>
		/// <param name="filename">Name of the file to load</param>
		/// <param name="forceGenerateSourceMap">
		/// <c>true</c> to re-transform the file if a cached version with no source map is available
		/// </param>
		/// <returns>JavaScript and source map</returns>
		JavaScriptWithSourceMap TransformFileWithSourceMap(
			string filename, 
			bool forceGenerateSourceMap = false
		);

		/// <summary>
		/// Transforms JavaScript via Babel. The result is not cached. Use 
		/// <see cref="TransformFile"/> if loading from a file since this will cache the result.
		/// </summary>
		/// <param name="input">JavaScript</param>
		/// <param name="filename">Name of the file being transformed</param>
		/// <returns>JavaScript</returns>
		string Transform(string input, string filename = "unknown.jsx");

		/// <summary>
		/// Transforms JavaScript via Babel and also returns a source map to map the compiled
		/// source to the original version. The result is not cached.
		/// </summary>
		/// <param name="input">JavaScript</param>
		/// <param name="filename">Name of the file being transformed</param>
		/// <returns>JavaScript and source map</returns>
		JavaScriptWithSourceMap TransformWithSourceMap(string input, string filename = "unknown");

		/// <summary>
		/// Transforms JavaScript via Babel and saves the result into a ".generated.js" file 
		/// alongside the original file.
		/// </summary>
		/// <param name="filename">Name of the file to load</param>
		/// <returns>File contents</returns>
		string TransformAndSaveFile(string filename);

		/// <summary>
		/// Returns the path the specified file's compilation will be cached to
		/// </summary>
		/// <param name="path">Path of the input file</param>
		/// <returns>Output path of the compiled file</returns>
		string GetOutputPath(string path);

		/// <summary>
		/// Returns the path the specified file's source map will be cached to if
		/// <see cref="TransformAndSaveFile"/> is called.
		/// </summary>
		/// <param name="path">Path of the input file</param>
		/// <returns>Output path of the source map</returns>
		string GetSourceMapOutputPath(string path);
	}
}
