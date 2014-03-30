/*
 *  Copyright (c) 2014, Facebook, Inc.
 *  All rights reserved.
 *
 *  This source code is licensed under the BSD-style license found in the
 *  LICENSE file in the root directory of this source tree. An additional grant 
 *  of patent rights can be found in the PATENTS file in the same directory.
 */

using System;
using Cassette.BundleProcessing;
using Cassette.Scripts;

namespace Cassette.React
{
	/// <summary>
	/// Handles processing of script bundles in Cassette. Adds a <see cref="JsxCompiler" />
	/// for all .jsx files.
	/// </summary>
	public class JsxBundleProcessor : IBundleProcessor<ScriptBundle>
	{
		private readonly CassetteSettings _settings;

		/// <summary>
		/// Initializes a new instance of the <see cref="JsxBundleProcessor"/> class.
		/// </summary>
		/// <param name="settings">Cassette settings.</param>
		public JsxBundleProcessor(CassetteSettings settings)
		{
			_settings = settings;
		}

		/// <summary>
		/// Processes the specified bundle. Adds a <see cref="JsxCompiler"/> for all .jsx files.
		/// </summary>
		/// <param name="bundle">The bundle.</param>
		public void Process(ScriptBundle bundle)
		{
			foreach (var asset in bundle.Assets)
			{
				if (asset.Path.EndsWith(".jsx", StringComparison.InvariantCultureIgnoreCase))
				{
					asset.AddAssetTransformer(
						new CompileAsset(new JsxCompiler(), _settings.SourceDirectory)
					);
				}
			}
		}
	}
}
