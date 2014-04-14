/*
 *  Copyright (c) 2014, Facebook, Inc.
 *  All rights reserved.
 *
 *  This source code is licensed under the BSD-style license found in the
 *  LICENSE file in the root directory of this source tree. An additional grant 
 *  of patent rights can be found in the PATENTS file in the same directory.
 */

using System.Linq;
using React;

namespace Cassette.React
{
	/// <summary>
	/// Handles compilation of JSX in Cassette
	/// </summary>
	public class JsxCompiler : ICompiler
	{
		private readonly IReactEnvironment _environment;

		/// <summary>
		/// Initializes a new instance of the <see cref="JsxCompiler"/> class.
		/// </summary>
		/// <param name="environment">The ReactJS.NET environment</param>
		public JsxCompiler(IReactEnvironment environment)
		{
			_environment = environment;
		}

		/// <summary>
		/// Compiles the specified JSX file into JavaScript
		/// </summary>
		/// <param name="source">The source.</param>
		/// <param name="context">The context.</param>
		/// <returns>JavaScript</returns>
		public CompileResult Compile(string source, CompileContext context)
		{
			var output = _environment.JsxTransformer.TransformJsx(source);
			return new CompileResult(output, Enumerable.Empty<string>());
		}
	}
}
