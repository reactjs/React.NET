/*
 *  Copyright (c) 2014-Present, Facebook, Inc.
 *  All rights reserved.
 *
 *  This source code is licensed under the BSD-style license found in the
 *  LICENSE file in the root directory of this source tree. An additional grant 
 *  of patent rights can be found in the PATENTS file in the same directory.
 */

using System.Collections.Generic;
#if OWIN
using Microsoft.Owin.StaticFiles;
#else
using Microsoft.AspNetCore.Builder;
#endif

#if OWIN
namespace React.Owin
#else
namespace React.AspNet
#endif
{
	/// <summary>
	/// Options for serving JavaScript files transformed via Babel.
	/// </summary>
	public class BabelFileOptions
	{
		/// <summary>
		/// Collection of extensions that will be handled. Defaults to ".jsx" and ".js".
		/// </summary>
		public IEnumerable<string> Extensions { get; set; }

		/// <summary>
		/// Options for static file middleware used to serve JavaScript files.
		/// </summary>
		public StaticFileOptions StaticFileOptions { get; set; }

		/// <summary>
		/// Creates a new instance of the <see cref="BabelFileOptions"/> class.
		/// </summary>
		public BabelFileOptions()
		{
			Extensions = new[] { ".jsx" };
			StaticFileOptions = new StaticFileOptions();
		}
	}
}
