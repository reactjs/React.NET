/*
 *  Copyright (c) 2014-2015, Facebook, Inc.
 *  All rights reserved.
 *
 *  This source code is licensed under the BSD-style license found in the
 *  LICENSE file in the root directory of this source tree. An additional grant 
 *  of patent rights can be found in the PATENTS file in the same directory.
 */

using System;
using System.Security.Cryptography;
using System.Text;

namespace React
{
	/// <summary>
	/// Handles calculating a hash value for validating a file-based cache.
	/// </summary>
	public class FileCacheHash : IFileCacheHash
	{
		/// <summary>
		/// Prefix used for hash line in transformed file. Used for caching.
		/// </summary>
		private const string HASH_PREFIX = "// @hash v3-";

		/// <summary>
		/// Algorithm for calculating file hashes
		/// </summary>
		private readonly HashAlgorithm _hash = SHA1.Create("System.Security.Cryptography.SHA1Cng");

		/// <summary>
		/// Calculates a hash for the specified input
		/// </summary>
		/// <param name="input">Input string</param>
		/// <returns>Hash of the input</returns>
		public string CalculateHash(string input)
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
		public virtual bool ValidateHash(string cacheContents, string hash)
		{
			if (string.IsNullOrWhiteSpace(cacheContents))
			{
				return false;
			}

			// Check if first line is hash
			var firstLineBreak = cacheContents.IndexOfAny(new[] { '\r', '\n' });
			if (firstLineBreak == -1)
			{
				return false;
			}
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
		/// Prepends the hash prefix to the hash
		/// </summary>
		/// <param name="hash">Hash to prepend prefix to</param>
		/// <returns>Hash with prefix</returns>
		public virtual string AddPrefix(string hash)
		{
			return HASH_PREFIX + hash;
		}
	}
}