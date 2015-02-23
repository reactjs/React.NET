/*
 *  Copyright (c) 2014-2015, Facebook, Inc.
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
using Microsoft.AspNet.StaticFiles;
#endif

#if OWIN
namespace React.Owin
#else
namespace React.AspNet5
#endif
{
	/// <summary>
	/// Options for serving JSX files.
	/// </summary>
	public class JsxFileOptions
	{
		/// <summary>
		/// Collection of extensions that will be treated as JSX files. Defaults to ".jsx" and ".js".
		/// </summary>
		public IEnumerable<string> Extensions { get; set; }

		/// <summary>
		/// Options for static file middleware used to server JSX files.
		/// </summary>
		public StaticFileOptions StaticFileOptions { get; set; }

		/// <summary>
		/// Creates a new instance of the <see cref="JsxFileOptions"/> class.
		/// </summary>
		public JsxFileOptions()
		{
			Extensions = new[] { ".jsx", ".js" };
			StaticFileOptions = new StaticFileOptions();
		}
	}
}