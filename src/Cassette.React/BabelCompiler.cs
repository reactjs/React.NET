/*
 *  Copyright (c) 2014-Present, Facebook, Inc.
 *  All rights reserved.
 *
 *  This source code is licensed under the BSD-style license found in the
 *  LICENSE file in the root directory of this source tree. An additional grant 
 *  of patent rights can be found in the PATENTS file in the same directory.
 */

using System.IO;
using System.Linq;
using React;

namespace Cassette.React
{
	/// <summary>
	/// Handles compilation of JavaScript files via Babel in Cassette
	/// </summary>
	public class BabelCompiler : ICompiler
	{
		private readonly IReactEnvironment _environment;

		/// <summary>
		/// Initializes a new instance of the <see cref="BabelCompiler"/> class.
		/// </summary>
		/// <param name="environment">The ReactJS.NET environment</param>
		public BabelCompiler(IReactEnvironment environment)
		{
			_environment = environment;
		}

		/// <summary>
		/// Compiles the specified JavaScript file via Babel
		/// </summary>
		/// <param name="source">The source.</param>
		/// <param name="context">The context.</param>
		/// <returns>JavaScript</returns>
		public CompileResult Compile(string source, CompileContext context)
		{
			var output = _environment.Babel.Transform(
				source, 
				Path.GetFileName(context.SourceFilePath)
			);
			return new CompileResult(output, Enumerable.Empty<string>());
		}
	}
}
