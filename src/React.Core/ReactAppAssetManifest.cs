/*
 * Copyright (c) Facebook, Inc. and its affiliates.
 *
 * This source code is licensed under the MIT license found in the
 * LICENSE file in the root directory of this source tree.
 */

using System.Collections.Generic;

namespace React
{
	internal class ReactAppAssetManifest
	{
		public Dictionary<string, string> Files { get; set; }
		public List<string> Entrypoints { get; set; }
	}

}
