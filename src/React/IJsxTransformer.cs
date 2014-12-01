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
		/// <param name="useHarmony"><c>true</c> if support for ES6 syntax should be enabled</param>
		/// <param name="stripTypes">
		/// Whether Flow types should be stripped out. Defaults to the value set in the site
		/// configuration.
		/// </param>
		/// <returns>JavaScript</returns>
		string TransformJsxFile(string filename, bool? useHarmony = null, bool? stripTypes = null);

		/// <summary>
		/// Transforms a JSX file to regular JavaScript and also returns a source map to map the
		/// compiled source to the original version. Results of the JSX to JavaScript 
		/// transformation are cached.
		/// </summary>
		/// <param name="filename">Name of the file to load</param>
		/// <param name="forceGenerateSourceMap">
		/// <c>true</c> to re-transform the file if a cached version with no source map is available
		/// </param>
		/// <param name="useHarmony"><c>true</c> if support for ES6 syntax should be enabled</param>
		/// <param name="stripTypes">
		/// Whether Flow types should be stripped out. Defaults to the value set in the site
		/// configuration.
		/// </param>
		/// <returns>JavaScript and source map</returns>
		JavaScriptWithSourceMap TransformJsxFileWithSourceMap(
			string filename, 
			bool forceGenerateSourceMap = false, 
			bool? useHarmony = null, 
			bool? stripTypes = null
		);

		/// <summary>
		/// Transforms JSX into regular JavaScript. The result is not cached. Use 
		/// <see cref="TransformJsxFile"/> if loading from a file since this will cache the result.
		/// </summary>
		/// <param name="input">JSX</param>
		/// <param name="useHarmony"><c>true</c> if support for ES6 syntax should be enabled</param>
		/// <param name="stripTypes">
		/// Whether Flow types should be stripped out. Defaults to the value set in the site
		/// configuration.
		/// </param>
		/// <returns>JavaScript</returns>
		string TransformJsx(string input, bool? useHarmony = null, bool? stripTypes = null);
		
		/// <summary>
		/// Transforms JSX to regular JavaScript and also returns a source map to map the compiled
		/// source to the original version. The result is not cached.
		/// </summary>
		/// <param name="input">JSX</param>
		/// <param name="useHarmony"><c>true</c> if support for ES6 syntax should be enabled</param>
		/// <param name="stripTypes">
		/// Whether Flow types should be stripped out. Defaults to the value set in the site
		/// configuration.
		/// </param>
		/// <returns>JavaScript and source map</returns>
		JavaScriptWithSourceMap TransformJsxWithSourceMap(string input, bool? useHarmony = null, bool? stripTypes = null);

		/// <summary>
		/// Transforms a JSX file to JavaScript, and saves the result into a ".generated.js" file 
		/// alongside the original file.
		/// </summary>
		/// <param name="filename">Name of the file to load</param>
		/// <param name="useHarmony"><c>true</c> if support for ES6 syntax should be enabled</param>
		/// <param name="stripTypes">
		/// Whether Flow types should be stripped out. Defaults to the value set in the site
		/// configuration.
		/// </param>
		/// <returns>File contents</returns>
		string TransformAndSaveJsxFile(string filename, bool? useHarmony = null, bool? stripTypes = null);

		/// <summary>
		/// Returns the path the specified JSX file's compilation will be cached to
		/// </summary>
		/// <param name="path">Path of the JSX file</param>
		/// <returns>Output path of the compiled file</returns>
		string GetJsxOutputPath(string path);

		/// <summary>
		/// Returns the path the specified JSX file's source map will be cached to if
		/// <see cref="TransformAndSaveJsxFile"/> is called.
		/// </summary>
		/// <param name="path">Path of the JSX file</param>
		/// <returns>Output path of the source map</returns>
		string GetSourceMapOutputPath(string path);
	}
}