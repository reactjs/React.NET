/*
 *  Copyright (c) 2014, Facebook, Inc.
 *  All rights reserved.
 *
 *  This source code is licensed under the BSD-style license found in the
 *  LICENSE file in the root directory of this source tree. An additional grant 
 *  of patent rights can be found in the PATENTS file in the same directory.
 */

using System;
using System.IO;
using React.Exceptions;

namespace React
{
	/// <summary>
	/// Handles compiling JSX to JavaScript.
	/// </summary>
	public class JsxTransformer : IJsxTransformer
	{
		/// <summary>
		/// Cache key for JSX to JavaScript compilation
		/// </summary>
		private const string JSX_CACHE_KEY = "JSX_{0}";
		/// <summary>
		/// Suffix to append to compiled files
		/// </summary>
		private const string COMPILED_FILE_SUFFIX = ".generated.js";

		/// <summary>
		/// Environment this JSX Transformer has been created in
		/// </summary>
		private readonly IReactEnvironment _environment;
		/// <summary>
		/// Cache used for storing compiled JSX
		/// </summary>
		private readonly ICache _cache;
		/// <summary>
		/// File system wrapper
		/// </summary>
		private readonly IFileSystem _fileSystem;
		/// <summary>
		/// Hash algorithm for file-based cache
		/// </summary>
		private readonly IFileCacheHash _fileCacheHash;
		/// <summary>
		/// Site-wide configuration
		/// </summary>
		private readonly IReactSiteConfiguration _config;

		/// <summary>
		/// Initializes a new instance of the <see cref="JsxTransformer"/> class.
		/// </summary>
		/// <param name="environment">The ReactJS.NET environment</param>
		/// <param name="cache">The cache to use for JSX compilation</param>
		/// <param name="fileSystem">File system wrapper</param>
		/// <param name="fileCacheHash">Hash algorithm for file-based cache</param>
		/// <param name="siteConfig">Site-wide configuration</param>
		public JsxTransformer(IReactEnvironment environment, ICache cache, IFileSystem fileSystem, IFileCacheHash fileCacheHash, IReactSiteConfiguration siteConfig)
		{
			_environment = environment;
			_cache = cache;
			_fileSystem = fileSystem;
			_fileCacheHash = fileCacheHash;
			_config = siteConfig;
		}

		/// <summary>
		/// Transforms a JSX file. Results of the JSX to JavaScript transformation are cached.
		/// </summary>
		/// <param name="filename">Name of the file to load</param>
		/// <param name="useHarmony"><c>true</c> if support for es6 syntax should be rewritten.</param>
		/// <returns>JavaScript</returns>
		public string TransformJsxFile(string filename, bool? useHarmony = null)
		{
			var fullPath = _fileSystem.MapPath(filename);

			// 1. Check in-memory cache
			return _cache.GetOrInsert(
				key: string.Format(JSX_CACHE_KEY, filename),
				slidingExpiration: TimeSpan.FromMinutes(30),
				cacheDependencyFiles: new[] { fullPath },
				getData: () =>
				{
					// 2. Check on-disk cache
					var contents = _fileSystem.ReadAsString(filename);
					var hash = _fileCacheHash.CalculateHash(contents);

					var cacheFilename = GetJsxOutputPath(filename);
					if (_fileSystem.FileExists(cacheFilename))
					{
						var cacheContents = _fileSystem.ReadAsString(cacheFilename);
						if (_fileCacheHash.ValidateHash(cacheContents, hash))
						{
							// Cache is valid :D
							return cacheContents;
						}
					}

					// 3. Not cached, perform the transformation
					try
					{
						return TransformJsxWithHeader(contents, hash, useHarmony);
					}
					catch (JsxException ex)
					{
						// Add the filename to the error message
						throw new JsxException(string.Format(
							"In file \"{0}\": {1}",
							filename,
							ex.Message
						));
					}
				}
			);
		}

		/// <summary>
		/// Transforms JSX into regular JavaScript, and prepends a header used for caching 
		/// purposes.
		/// </summary>
		/// <param name="contents">Contents of the input file</param>
		/// <param name="hash">Hash of the input. If null, it will be calculated</param>
		/// <param name="useHarmony"><c>true</c> if support for es6 syntax should be rewritten.</param>
		/// <returns>JavaScript</returns>
		private string TransformJsxWithHeader(string contents, string hash = null, bool? useHarmony = null)
		{
			if (string.IsNullOrEmpty(hash))
			{
				hash = _fileCacheHash.CalculateHash(contents);
			}
			return GetFileHeader(hash) + TransformJsx(contents, useHarmony);
		}

		/// <summary>
		/// Transforms JSX into regular JavaScript. The result is not cached. Use 
		/// <see cref="TransformJsxFile"/> if loading from a file since this will cache the result.
		/// </summary>
		/// <param name="input">JSX</param>
		/// <param name="useHarmony"><c>true</c> if support for es6 syntax should be rewritten.</param>
		/// <returns>JavaScript</returns>
		public string TransformJsx(string input, bool? useHarmony = null)
		{
			EnsureJsxTransformerSupported();
			try
			{
				var output = _environment.ExecuteWithLargerStackIfRequired<string>(
					"ReactNET_transform",
					input,
					useHarmony ?? _config.UseHarmony
				);
				return output;
			}
			catch (Exception ex)
			{
				throw new JsxException(ex.Message, ex);
			}
		}

		/// <summary>
		/// Transforms JSX to regular JavaScript and also returns a source map to map the compiled
		/// source to the original version. The result is not cached.
		/// </summary>
		/// <param name="input">JSX</param>
		/// <param name="useHarmony"><c>true</c> if support for ES6 syntax should be enabled</param>
		/// <returns>JavaScript and source map</returns>
		public JavaScriptWithSourceMap TransformJsxWithSourceMap(string input, bool? useHarmony)
		{
			EnsureJsxTransformerSupported();
			try
			{
				return _environment.ExecuteWithLargerStackIfRequired<JavaScriptWithSourceMap>(
					"ReactNET_transform_sourcemap",
					input,
					useHarmony ?? _config.UseHarmony
				);
			}
			catch (Exception ex)
			{
				throw new JsxException(ex.Message, ex);
			}
		}

		/// <summary>
		/// Gets the header prepended to JSX transformed files. Contains a hash that is used to 
		/// validate the cache.
		/// </summary>
		/// <param name="hash">Hash of the input</param>
		/// <returns>Header for the cache</returns>
		private string GetFileHeader(string hash)
		{
			return string.Format(
@"{0}
// Automatically generated by ReactJS.NET. Do not edit, your changes will be overridden.
// Version: {1}
// Generated at: {2}
///////////////////////////////////////////////////////////////////////////////
", _fileCacheHash.AddPrefix(hash), _environment.Version, DateTime.Now);
		}

		/// <summary>
		/// Returns the path the specified JSX file's compilation will be cached to if 
		/// <see cref="TransformAndSaveJsxFile" /> is called.
		/// </summary>
		/// <param name="path">Path of the JSX file</param>
		/// <returns>Output path of the compiled file</returns>
		public string GetJsxOutputPath(string path)
		{
			return Path.Combine(
				Path.GetDirectoryName(path),
				Path.GetFileNameWithoutExtension(path) + COMPILED_FILE_SUFFIX
			);
		}

		/// <summary>
		/// Transforms a JSX file to JavaScript, and saves the result into a ".generated.js" file 
		/// alongside the original file.
		/// </summary>
		/// <param name="filename">Name of the file to load</param>
		/// <param name="useHarmony"><c>true</c> if support for es6 syntax should be rewritten.</param>
		/// <returns>File contents</returns>
		public string TransformAndSaveJsxFile(string filename, bool? useHarmony = null)
		{
			var outputPath = GetJsxOutputPath(filename);
			var contents = _fileSystem.ReadAsString(filename);
			var result = TransformJsxWithHeader(contents, useHarmony: useHarmony);
			_fileSystem.WriteAsString(outputPath, result);
			return outputPath;
		}

		/// <summary>
		/// Ensures that the current JavaScript engine supports JSXTransformer. Throws an exception
		/// if it doesn't.
		/// </summary>
		private void EnsureJsxTransformerSupported()
		{
			if (!_environment.EngineSupportsJsxTransformer)
			{
				throw new JsxUnsupportedEngineException();
			}
		}
	}
}
