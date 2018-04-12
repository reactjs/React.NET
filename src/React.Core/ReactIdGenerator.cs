/*
 *  Copyright (c) 2016-Present, Facebook, Inc.
 *  All rights reserved.
 *
 *  This source code is licensed under the BSD-style license found in the
 *  LICENSE file in the root directory of this source tree. An additional grant 
 *  of patent rights can be found in the PATENTS file in the same directory.
 */

using System;
using System.Threading;

namespace React
{
	/// <summary>
	/// React ID generator.
	/// </summary>
	public class ReactIdGenerator : IReactIdGenerator
	{
		private static readonly string _encode32Chars = "0123456789ABCDEFGHIJKLMNOPQRSTUV";

		private static long _random = DateTime.UtcNow.Ticks;

		private static readonly char[] reactPrefix = "react_".ToCharArray();

		/// <summary>
		/// "react_".Length = 6 + 13 random symbols
		/// </summary>
		private const int reactIdLength = 19;

		[ThreadStatic]
		private static char[] _chars;

		/// <summary>
		/// Returns a short react identifier starts with "react_".
		/// </summary>
		/// <returns></returns>
		public string Generate()
		{
			var chars = _chars;
			if (chars == null)
			{
				_chars = chars = new char[reactIdLength];
				Array.Copy(reactPrefix, 0, chars, 0, reactPrefix.Length);
			}

			var id = Interlocked.Increment(ref _random);

			// from 6 because  "react_".Length == 6, _encode32Chars.Length == 32 (base32), 
			// base32 characters are 5 bits in length and from long (64 bits) we can get 13 symbols
			chars[6] = _encode32Chars[(int)(id >> 60) & 31];
			chars[7] = _encode32Chars[(int)(id >> 55) & 31];
			chars[8] = _encode32Chars[(int)(id >> 50) & 31];
			chars[9] = _encode32Chars[(int)(id >> 45) & 31];
			chars[10] = _encode32Chars[(int)(id >> 40) & 31];
			chars[11] = _encode32Chars[(int)(id >> 35) & 31];
			chars[12] = _encode32Chars[(int)(id >> 30) & 31];
			chars[13] = _encode32Chars[(int)(id >> 25) & 31];
			chars[14] = _encode32Chars[(int)(id >> 20) & 31];
			chars[15] = _encode32Chars[(int)(id >> 15) & 31];
			chars[16] = _encode32Chars[(int)(id >> 10) & 31];
			chars[17] = _encode32Chars[(int)(id >> 5) & 31];
			chars[18] = _encode32Chars[(int)id & 31];

			return new string(chars, 0, reactIdLength);
		}
	}
}
