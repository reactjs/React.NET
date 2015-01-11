/*
 *  Copyright (c) 2014-2015, Facebook, Inc.
 *  All rights reserved.
 *
 *  This source code is licensed under the BSD-style license found in the
 *  LICENSE file in the root directory of this source tree. An additional grant 
 *  of patent rights can be found in the PATENTS file in the same directory.
 */

using Cassette.BundleProcessing;
using Cassette.Scripts;

namespace Cassette.React
{
	/// <summary>
	/// Inserts the <see cref="JsxBundleProcessor" /> into the script bundle pipeline
	/// </summary>
	public class InsertIntoPipelineJsxBundleProcessor : IBundlePipelineModifier<ScriptBundle>
	{
		/// <summary>
		/// Modifies the specified pipeline.
		/// </summary>
		/// <param name="pipeline">The pipeline.</param>
		/// <returns>The pipeline</returns>
		public IBundlePipeline<ScriptBundle> Modify(IBundlePipeline<ScriptBundle> pipeline)
		{
			var index = pipeline.IndexOf<ParseJavaScriptReferences>();
			pipeline.Insert<JsxBundleProcessor>(index + 1);
			return pipeline;
		}
	}
}
