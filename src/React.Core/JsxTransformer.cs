/*
 *  Copyright (c) 2014-2015, Facebook, Inc.
 *  All rights reserved.
 *
 *  This source code is licensed under the BSD-style license found in the
 *  LICENSE file in the root directory of this source tree. An additional grant 
 *  of patent rights can be found in the PATENTS file in the same directory.
 */

using System;
using System.Collections.Generic;
using System.Diagnostics;
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
		protected const string JSX_CACHE_KEY = "JSX_v3_{0}";
		/// <summary>
		/// Suffix to append to compiled files
		/// </summary>
		protected const string COMPILED_FILE_SUFFIX = ".generated.js";
		/// <summary>
		/// Suffix to append to source map files
		/// </summary>
		protected const string SOURE_MAP_FILE_SUFFIX = ".map";
		/// <summary>
		/// Number of lines in the header prepended to compiled JSX files.
		/// </summary>
		protected const int LINES_IN_HEADER = 5;

		/// <summary>
		/// Environment this JSX Transformer has been created in
		/// </summary>
		protected readonly IReactEnvironment _environment;
		/// <summary>
		/// Cache used for storing compiled JSX
		/// </summary>
		protected readonly ICache _cache;
		/// <summary>
		/// File system wrapper
		/// </summary>
		protected readonly IFileSystem _fileSystem;
		/// <summary>
		/// Hash algorithm for file-based cache
		/// </summary>
		protected readonly IFileCacheHash _fileCacheHash;
		/// <summary>
		/// Site-wide configuration
		/// </summary>
		protected readonly IReactSiteConfiguration _config;
		/// <summary>
		/// The serialized Babel configuration
		/// </summary>
		protected readonly string _babelConfig; 

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
			_babelConfig = siteConfig.BabelConfig.Serialize();
		}

		/// <summary>
		/// Transforms a JSX file. Results of the JSX to JavaScript transformation are cached.
		/// </summary>
		/// <param name="filename">Name of the file to load</param>
		/// <returns>JavaScript</returns>
		public virtual string TransformJsxFile(string filename)
		{
			return TransformJsxFileWithSourceMap(filename, false).Code;
		}

		/// <summary>
		/// Transforms a JSX file to regular JavaScript and also returns a source map to map the
		/// compiled source to the original version. Results of the JSX to JavaScript 
		/// transformation are cached.
		/// </summary>
		/// <param name="filename">Name of the file to load</param>
		/// <param name="forceGenerateSourceMap">
		/// <c>true</c> to re-transform the file if a cached version with no source map is available
		/// </param>
		/// <returns>JavaScript and source map</returns>
		public virtual JavaScriptWithSourceMap TransformJsxFileWithSourceMap(
			string filename,
			bool forceGenerateSourceMap = false
		)
		{
			var cacheKey = string.Format(JSX_CACHE_KEY, filename);

			// 1. Check in-memory cache. We need to invalidate any in-memory cache if there's no 
			// source map cached and forceGenerateSourceMap is true.
			var cached = _cache.Get<JavaScriptWithSourceMap>(cacheKey);
			var cacheIsValid = cached != null && (!forceGenerateSourceMap || cached.SourceMap != null);
			if (cacheIsValid)
			{
				return cached;
			}

			// 2. Check on-disk cache
			var contents = _fileSystem.ReadAsString(filename);
			var hash = _fileCacheHash.CalculateHash(contents);
			var output = LoadJsxFromFileCache(filename, hash, forceGenerateSourceMap);
			if (output == null)
			{
				// 3. Not cached, perform the transformation
				try
				{
					output = TransformJsxWithHeader(filename, contents, hash);
				}
				catch (JsxException ex)
				{
					// Add the filename to the error message
					throw new JsxException(string.Format(
						"In file \"{0}\": {1}",
						filename,
						ex.Message
					), ex);
				}
			}

			// Cache the result from above (either disk cache or live transformation) to memory
			var fullPath = _fileSystem.MapPath(filename);
			_cache.Set(
				cacheKey,
				output,
				slidingExpiration: TimeSpan.FromMinutes(30),
				cacheDependencyFiles: new[] { fullPath }
			);
			return output;
		}

		/// <summary>
		/// Loads a transformed JSX file from the disk cache. If the cache is invalid or there is
		/// no cached version, returns <c>null</c>.
		/// </summary>
		/// <param name="filename">Name of the file to load</param>
		/// /// <param name="hash">Hash of the input file, to validate the cache</param>
		/// <param name="forceGenerateSourceMap">
		/// <c>true</c> to re-transform the file if a cached version with no source map is available
		/// </param>
		/// <returns></returns>
		protected virtual JavaScriptWithSourceMap LoadJsxFromFileCache(string filename, string hash, bool forceGenerateSourceMap)
		{
			var cacheFilename = GetJsxOutputPath(filename);
			if (!_fileSystem.FileExists(cacheFilename))
			{
				// Cache file doesn't exist on disk
				return null;
			}
			var cacheContents = _fileSystem.ReadAsString(cacheFilename);
			if (!_fileCacheHash.ValidateHash(cacheContents, hash))
			{
				// Hash of the cache is invalid (file changed since the time the cache was written).
				return null;
			}
				
			// Cache is valid :D
			// See if we have a source map cached alongside the file
			SourceMap sourceMap = null;
			var sourceMapFilename = GetSourceMapOutputPath(filename);
			if (_fileSystem.FileExists(sourceMapFilename))
			{
				try
				{
					var sourceMapString = _fileSystem.ReadAsString(sourceMapFilename);
					if (!string.IsNullOrEmpty(sourceMapString))
					{
						sourceMap = SourceMap.FromJson(sourceMapString);
					}
				}
				catch (Exception e)
				{
					// Just ignore it
					Trace.WriteLine("Error reading source map file: " + e.Message);
				}
			}
			
			// If forceGenerateSourceMap is true, we need to explicitly ignore this cached version
			// if there's no source map
			if (forceGenerateSourceMap && sourceMap == null)
			{
				return null;
			}

			return new JavaScriptWithSourceMap
			{
				Code = cacheContents,
				SourceMap = sourceMap,
				Hash = hash,
			};
		}

		/// <summary>
		/// Transforms JSX into regular JavaScript, and prepends a header used for caching 
		/// purposes.
		/// </summary>
		/// <param name="filename">Name of the file being transformed</param>
		/// <param name="contents">Contents of the input file</param>
		/// <param name="hash">Hash of the input. If null, it will be calculated</param>
		/// <returns>JavaScript</returns>
		protected virtual JavaScriptWithSourceMap TransformJsxWithHeader(
			string filename, 
			string contents, 
			string hash = null
		)
		{
			if (string.IsNullOrEmpty(hash))
			{
				hash = _fileCacheHash.CalculateHash(contents);
			}
			var header = GetFileHeader(hash);
			var result = TransformJsxWithSourceMap(header + contents, filename);
			result.Hash = hash;
			if (result.SourceMap != null)
			{
				// Insert original source into source map so the browser doesn't have to do a second
				// request for it. The newlines in the beginning are a hack so the line numbers line
				// up (as the original file doesn't have the header the transformed file includes).
				result.SourceMap.Sources = new[] { Path.GetFileName(filename) + ".source" };
				result.SourceMap.SourcesContent = new[] { new string('\n', LINES_IN_HEADER) + contents };
				result.SourceMap.File = null;
			}
			
			return result;
		}

		/// <summary>
		/// Transforms JSX into regular JavaScript. The result is not cached. Use 
		/// <see cref="TransformJsxFile"/> if loading from a file since this will cache the result.
		/// </summary>
		/// <param name="input">JSX</param>
		/// <param name="filename">Name of the file being transformed</param>
		/// <returns>JavaScript</returns>
		public virtual string TransformJsx(string input, string filename = "unknown")
		{
			EnsureBabelLoaded();
			try
			{
				var output = _environment.ExecuteWithLargerStackIfRequired<string>(
					"ReactNET_transform",
					input,
					_babelConfig,
					filename
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
		/// <param name="filename">Name of the file being transformed</param>
		/// <returns>JavaScript and source map</returns>
		public virtual JavaScriptWithSourceMap TransformJsxWithSourceMap(
			string input,
			string filename = "unknown"
		)
		{
			EnsureBabelLoaded();
			try
			{
				return _environment.ExecuteWithLargerStackIfRequired<JavaScriptWithSourceMap>(
					"ReactNET_transform_sourcemap",
					input,
					_babelConfig,
					filename
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
		protected virtual string GetFileHeader(string hash)
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
		public virtual string GetJsxOutputPath(string path)
		{
			return Path.Combine(
				Path.GetDirectoryName(path),
				Path.GetFileNameWithoutExtension(path) + COMPILED_FILE_SUFFIX
			);
		}

		/// <summary>
		/// Returns the path the specified JSX file's source map will be cached to if
		/// <see cref="TransformAndSaveJsxFile"/> is called.
		/// </summary>
		/// <param name="path">Path of the JSX file</param>
		/// <returns>Output path of the source map</returns>
		public virtual string GetSourceMapOutputPath(string path)
		{
			return GetJsxOutputPath(path) + SOURE_MAP_FILE_SUFFIX;
		}

		/// <summary>
		/// Transforms a JSX file to JavaScript, and saves the result into a ".generated.js" file 
		/// alongside the original file.
		/// </summary>
		/// <param name="filename">Name of the file to load</param>
		/// <returns>File contents</returns>
		public virtual string TransformAndSaveJsxFile(
			string filename
		)
		{
			var outputPath = GetJsxOutputPath(filename);
			var sourceMapPath = GetSourceMapOutputPath(filename);
			var contents = _fileSystem.ReadAsString(filename);
			var result = TransformJsxWithHeader(filename, contents, null);
			_fileSystem.WriteAsString(outputPath, result.Code);
			_fileSystem.WriteAsString(sourceMapPath, result.SourceMap == null ? string.Empty : result.SourceMap.ToJson());
			return outputPath;
		}

		/// <summary>
		/// Ensures that Babel has been loaded into the JavaScript engine.
		/// </summary>
		private void EnsureBabelLoaded()
		{
			if (!_config.LoadBabel)
			{
				throw new BabelNotLoadedException();
			}
		}
	}
}
