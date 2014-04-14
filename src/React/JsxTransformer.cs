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
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;
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
		/// Prefix used for hash line in transformed file. Used for caching.
		/// </summary>
		private const string HASH_PREFIX = "// @hash ";

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
		/// Althorithm for calculating file hashes
		/// </summary>
		private readonly HashAlgorithm _hash = MD5.Create();

		/// <summary>
		/// Initializes a new instance of the <see cref="JsxTransformer"/> class.
		/// </summary>
		/// <param name="environment">The ReactJS.NET environment</param>
		/// <param name="cache">The cache to use for JSX compilation</param>
		/// <param name="fileSystem">File system wrapper</param>
		public JsxTransformer(IReactEnvironment environment, ICache cache, IFileSystem fileSystem)
		{
			_environment = environment;
			_cache = cache;
			_fileSystem = fileSystem;
		}

		/// <summary>
		/// Transforms a JSX file. Results of the JSX to JavaScript transformation are cached.
		/// </summary>
		/// <param name="filename">Name of the file to load</param>
		/// <returns>JavaScript</returns>
		public string TransformJsxFile(string filename)
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
					var hash = CalculateHash(contents);

					var cacheFilename = GetJsxOutputPath(filename);
					if (_fileSystem.FileExists(cacheFilename))
					{
						var cacheContents = _fileSystem.ReadAsString(cacheFilename);
						if (ValidateHash(cacheContents, hash))
						{
							// Cache is valid :D
							return cacheContents;
						}
					}

					// 3. Not cached, perform the transformation
					return TransformJsxWithHeader(contents, hash);
				}
			);
		}

		/// <summary>
		/// Transforms JSX into regular JavaScript, and prepends a header used for caching 
		/// purposes.
		/// </summary>
		/// <param name="contents">Contents of the input file</param>
		/// <param name="hash">Hash of the input. If null, it will be calculated</param>
		/// <returns>JavaScript</returns>
		private string TransformJsxWithHeader(string contents, string hash = null)
		{
			if (string.IsNullOrEmpty(hash))
			{
				hash = CalculateHash(contents);
			}
			return GetFileHeader(hash) + TransformJsx(contents);
		}

		/// <summary>
		/// Transforms JSX into regular JavaScript. The result is not cached. Use 
		/// <see cref="TransformJsxFile"/> if loading from a file since this will cache the result.
		/// </summary>
		/// <param name="input">JSX</param>
		/// <returns>JavaScript</returns>
		public string TransformJsx(string input)
		{
			// Just return directly if there's no JSX annotation
			if (!input.Contains("@jsx"))
			{
				return input;
			}

			EnsureJsxTransformerSupported();
			try
			{
				var encodedInput = JsonConvert.SerializeObject(input);
				var output = _environment.ExecuteWithLargerStackIfRequired<string>(string.Format(
					"global.JSXTransformer.transform({0}).code",
					encodedInput
				));
				return output;
			}
			catch (Exception ex)
			{
				throw new JsxException(ex.Message, ex);
			}
		}

		/// <summary>
		/// Calculates a hash for the specified input
		/// </summary>
		/// <param name="input">Input string</param>
		/// <returns>Hash of the input</returns>
		private string CalculateHash(string input)
		{
			var inputBytes = Encoding.UTF8.GetBytes(input);
			var hash = _hash.ComputeHash(inputBytes);
			return BitConverter.ToString(hash).Replace("-", string.Empty);
		}

		/// <summary>
		/// Validates that the cache's hash is valid. This is used to ensure the input has not
		/// changed, and to invalidate the cache if so.
		/// </summary>
		/// <param name="cacheContents">Contents retrieved from cache</param>
		/// <param name="hash">Hash of the input</param>
		/// <returns><c>true</c> if the cache is still valid</returns>
		private bool ValidateHash(string cacheContents, string hash)
		{
			// Check if first line is hash
			var firstLineBreak = cacheContents.IndexOfAny(new[] { '\r', '\n' });
			var firstLine = cacheContents.Substring(0, firstLineBreak);
			if (!firstLine.StartsWith(HASH_PREFIX))
			{
				// Cache doesn't have hash - Err on the side of caution and invalidate it.
				return false;
			}
			var cacheHash = firstLine.Replace(HASH_PREFIX, string.Empty);
			return cacheHash == hash;
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
@"{0}{1}
// Automatically generated by ReactJS.NET. Do not edit, your changes will be overridden.
// Version: {2}
///////////////////////////////////////////////////////////////////////////////
", HASH_PREFIX, hash, _environment.Version);
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
		/// <returns>File contents</returns>
		public string TransformAndSaveJsxFile(string filename)
		{
			var outputPath = GetJsxOutputPath(filename);
			var contents = _fileSystem.ReadAsString(filename);
			var result = TransformJsxWithHeader(contents);
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
