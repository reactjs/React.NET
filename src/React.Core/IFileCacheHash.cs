/*
 * Copyright (c) Facebook, Inc. and its affiliates.
 *
 * This source code is licensed under the MIT license found in the
 * LICENSE file in the root directory of this source tree.
 */

namespace React
{
	/// <summary>
	/// Handles calculating a hash value for validating a file-based cache.
	/// </summary>
	public interface IFileCacheHash
	{
		/// <summary>
		/// Calculates a hash for the specified input
		/// </summary>
		/// <param name="input">Input string</param>
		/// <returns>Hash of the input</returns>
		string CalculateHash(string input);

		/// <summary>
		/// Validates that the cache's hash is valid. This is used to ensure the input has not
		/// changed, and to invalidate the cache if so.
		/// </summary>
		/// <param name="cacheContents">Contents retrieved from cache</param>
		/// <param name="hash">Hash of the input</param>
		/// <returns><c>true</c> if the cache is still valid</returns>
		bool ValidateHash(string cacheContents, string hash);

		/// <summary>
		/// Prepends the hash prefix to the hash
		/// </summary>
		/// <param name="hash">Hash to prepend prefix to</param>
		/// <returns>Hash with prefix</returns>
		string AddPrefix(string hash);
	}
}
