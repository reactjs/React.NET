/*
 *  Copyright (c) 2014, Facebook, Inc.
 *  All rights reserved.
 *
 *  This source code is licensed under the BSD-style license found in the
 *  LICENSE file in the root directory of this source tree. An additional grant 
 *  of patent rights can be found in the PATENTS file in the same directory.
 */

using System;
using System.Collections.Generic;

namespace React
{
	/// <summary>
	/// Represents the data contained in a source map
	/// </summary>
	[Serializable]
	public class SourceMap
	{
		/// <summary>
		/// Version number of the source map spec used to build this source map. Expected
		/// to be version 3.
		/// </summary>
		public int Version { get; set; }

		/// <summary>
		/// An optional name of the generated code that this source map is associated with.
		/// </summary>
		public string File { get; set; }

		/// <summary>
		/// An optional source root, useful for relocating source files on a server or
		/// removing repeated values in the <see cref="Sources"/> entry.  This value is 
		/// prepended to the individual entries in the <see cref="Sources"/> field.
		/// </summary>
		public string SourceRoot { get; set; }

		/// <summary>
		/// A list of original sources used by the <see cref="Mappings"/> entry.
		/// </summary>
		public IList<string> Sources { get; set; }

		/// <summary>
		/// An optional list of source content, useful when the <see cref="Sources"/> can't 
		/// be hosted. The contents are listed in the same order as the <see cref="Sources"/>. 
		/// <c>null</c> may be used if some original sources should be retrieved by name.
		/// </summary>
		public IList<string> SourcesContent { get; set; }

		/// <summary>
		/// A list of symbol names used by the <see cref="Mappings"/> entry.
		/// </summary>
		public IList<string> Names { get; set; }

		/// <summary>
		/// A string with the mapping data encoded in base 64 VLQ.
		/// </summary>
		public string Mappings { get; set; }
	}
}
