/*
 *  Copyright (c) 2014-Present, Facebook, Inc.
 *  All rights reserved.
 *
 *  This source code is licensed under the BSD-style license found in the
 *  LICENSE file in the root directory of this source tree. An additional grant 
 *  of patent rights can be found in the PATENTS file in the same directory.
 */

using System;
using System.Diagnostics;
using System.IO;
using React.Exceptions;

namespace React
{
	/// <summary>
	/// Handles compiling JavaScript files via Babel (http://babeljs.io/).
	/// </summary>
	public class Babel : IBabel
	{
		/// <summary>
		/// Cache key for JavaScript compilation
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
		/// Number of lines in the header prepended to compiled files.
		/// </summary>
		protected const int LINES_IN_HEADER = 5;

		/// <summary>
		/// Environment this transformer has been created in
		/// </summary>
		protected readonly IReactEnvironment _environment;
		/// <summary>
		/// Cache used for storing compiled JavaScript
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
		/// Initializes a new instance of the <see cref="Babel"/> class.
		/// </summary>
		/// <param name="environment">The ReactJS.NET environment</param>
		/// <param name="cache">The cache to use for compilation</param>
		/// <param name="fileSystem">File system wrapper</param>
		/// <param name="fileCacheHash">Hash algorithm for file-based cache</param>
		/// <param name="siteConfig">Site-wide configuration</param>
		public Babel(IReactEnvironment environment, ICache cache, IFileSystem fileSystem, IFileCacheHash fileCacheHash, IReactSiteConfiguration siteConfig)
		{
			_environment = environment;
			_cache = cache;
			_fileSystem = fileSystem;
			_fileCacheHash = fileCacheHash;
			_config = siteConfig;
			_babelConfig = siteConfig.BabelConfig.Serialize();
		}

		/// <summary>
		/// Transforms a JavaScript file. Results of the transformation are cached.
		/// </summary>
		/// <param name="filename">Name of the file to load</param>
		/// <returns>JavaScript</returns>
		public virtual string TransformFile(string filename)
		{
			return TransformFileWithSourceMap(filename, false).Code;
		}

		/// <summary>
		/// Transforms a JavaScript file via Babel and also returns a source map to map the
		/// compiled source to the original version. Results of the transformation are cached.
		/// </summary>
		/// <param name="filename">Name of the file to load</param>
		/// <param name="forceGenerateSourceMap">
		/// <c>true</c> to re-transform the file if a cached version with no source map is available
		/// </param>
		/// <returns>JavaScript and source map</returns>
		public virtual JavaScriptWithSourceMap TransformFileWithSourceMap(
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
			var output = LoadFromFileCache(filename, hash, forceGenerateSourceMap);
			if (output == null)
			{
				// 3. Not cached, perform the transformation
				try
				{
					output = TransformWithHeader(filename, contents, hash);
				}
				catch (BabelException ex)
				{
					// Add the filename to the error message
					throw new BabelException(string.Format(
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
		/// Loads a transformed JavaScript file from the disk cache. If the cache is invalid or there is
		/// no cached version, returns <c>null</c>.
		/// </summary>
		/// <param name="filename">Name of the file to load</param>
		/// /// <param name="hash">Hash of the input file, to validate the cache</param>
		/// <param name="forceGenerateSourceMap">
		/// <c>true</c> to re-transform the file if a cached version with no source map is available
		/// </param>
		/// <returns></returns>
		protected virtual JavaScriptWithSourceMap LoadFromFileCache(string filename, string hash, bool forceGenerateSourceMap)
		{
			var cacheFilename = GetOutputPath(filename);
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
		/// Transforms JavaScript via Babel, and prepends a header used for caching 
		/// purposes.
		/// </summary>
		/// <param name="filename">Name of the file being transformed</param>
		/// <param name="contents">Contents of the input file</param>
		/// <param name="hash">Hash of the input. If null, it will be calculated</param>
		/// <returns>JavaScript</returns>
		protected virtual JavaScriptWithSourceMap TransformWithHeader(
			string filename, 
			string contents, 
			string hash = null
		)
		{
			var result = TransformWithSourceMap(contents, filename);
			if (string.IsNullOrEmpty(hash))
			{
				hash = _fileCacheHash.CalculateHash(contents);
			}
			// Prepend header to generated code
			var header = GetFileHeader(hash, result.BabelVersion);
			result.Code = header + result.Code;
			result.Hash = hash;

			if (result.SourceMap != null && result.SourceMap.Mappings != null)
			{
				// Since we prepend a header to the code, the source map no longer matches up exactly
				// (it's off by the number of lines in the header). Fix this issue by adding five 
				// blank lines to the source map. This is kinda hacky but saves us having to load a
				// proper source map library. If this ever breaks, I'll replace it with actual proper
				// source map modification code (https://gist.github.com/Daniel15/4bdb15836bfd960c2956).
				result.SourceMap.Mappings = ";;;;;" + result.SourceMap.Mappings;
			}

			return result;
		}

		/// <summary>
		/// Transforms JavaScript via Babel. The result is not cached. Use 
		/// <see cref="TransformFile"/> if loading from a file since this will cache the result.
		/// </summary>
		/// <param name="input">JavaScript</param>
		/// <param name="filename">Name of the file being transformed</param>
		/// <returns>JavaScript</returns>
		public virtual string Transform(string input, string filename = "unknown")
		{
			try
			{
				var output = _environment.ExecuteWithBabel<string>(
					"ReactNET_transform",
					input,
					_babelConfig,
					filename
				);
				return output;
			}
			catch (Exception ex)
			{
				throw new BabelException(ex.Message, ex);
			}
		}

		/// <summary>
		/// Transforms JavaScript via Babel and also returns a source map to map the compiled
		/// source to the original version. The result is not cached.
		/// </summary>
		/// <param name="input">JavaScript</param>
		/// <param name="filename">Name of the file being transformed</param>
		/// <returns>JavaScript and source map</returns>
		public virtual JavaScriptWithSourceMap TransformWithSourceMap(
			string input,
			string filename = "unknown"
		)
		{
			try
			{
				return _environment.ExecuteWithBabel<JavaScriptWithSourceMap>(
					"ReactNET_transform_sourcemap",
					input,
					_babelConfig,
					filename
				);
			}
			catch (Exception ex)
			{
				throw new BabelException(ex.Message, ex);
			}
		}

		/// <summary>
		/// Gets the header prepended to transformed files. Contains a hash that is used to 
		/// validate the cache.
		/// </summary>
		/// <param name="hash">Hash of the input</param>
		/// <param name="babelVersion">Version of Babel used to perform this transformation</param>
		/// <returns>Header for the cache</returns>
		protected virtual string GetFileHeader(string hash, string babelVersion)
		{
			return string.Format(
@"{0}
// Automatically generated by ReactJS.NET. Do not edit, your changes will be overridden.
// Version: {1} with Babel {3}
// Generated at: {2}
///////////////////////////////////////////////////////////////////////////////
", _fileCacheHash.AddPrefix(hash), _environment.Version, DateTime.Now, babelVersion);
		}

		/// <summary>
		/// Returns the path the specified file's compilation will be cached to
		/// </summary>
		/// <param name="path">Path of the input file</param>
		/// <returns>Output path of the compiled file</returns>
		public virtual string GetOutputPath(string path)
		{
			return Path.Combine(
				Path.GetDirectoryName(path),
				Path.GetFileNameWithoutExtension(path) + COMPILED_FILE_SUFFIX
			);
		}

		/// <summary>
		/// Returns the path the specified file's source map will be cached to if
		/// <see cref="TransformAndSaveFile"/> is called.
		/// </summary>
		/// <param name="path">Path of the input file</param>
		/// <returns>Output path of the source map</returns>
		public virtual string GetSourceMapOutputPath(string path)
		{
			return GetOutputPath(path) + SOURE_MAP_FILE_SUFFIX;
		}

		/// <summary>
		/// Transforms JavaScript via Babel and saves the result into a ".generated.js" file 
		/// alongside the original file.
		/// </summary>
		/// <param name="filename">Name of the file to load</param>
		/// <returns>File contents</returns>
		public virtual string TransformAndSaveFile(
			string filename
		)
		{
			var outputPath = GetOutputPath(filename);
			var sourceMapPath = GetSourceMapOutputPath(filename);
			var contents = _fileSystem.ReadAsString(filename);
			var result = TransformWithHeader(filename, contents, null);
			_fileSystem.WriteAsString(outputPath, result.Code);
			_fileSystem.WriteAsString(sourceMapPath, result.SourceMap == null ? string.Empty : result.SourceMap.ToJson());
			return outputPath;
		}
	}
}
