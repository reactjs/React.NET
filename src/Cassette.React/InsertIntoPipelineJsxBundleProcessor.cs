/*
 * Copyright (c) Facebook, Inc. and its affiliates.
 *
 * This source code is licensed under the MIT license found in the
 * LICENSE file in the root directory of this source tree.
 */

using Cassette.BundleProcessing;
using Cassette.Scripts;

namespace Cassette.React
{
	/// <summary>
	/// Inserts the <see cref="BabelBundleProcessor" /> into the script bundle pipeline
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
			pipeline.Insert<BabelBundleProcessor>(index + 1);
			return pipeline;
		}
	}
}
