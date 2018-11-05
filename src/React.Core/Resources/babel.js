/**
 * Copyright (c) Facebook, Inc. and its affiliates.
 *
 * This source code is licensed under the MIT license found in the
 * LICENSE file in the root directory of this source tree.
 */

import {transform as babelTransform, version as babelVersion} from 'babel-standalone';

export function ReactNET_transform(input, babelConfig, filename) {
	babelConfig = {
		...JSON.parse(babelConfig),
		ast: false,
		filename,
	}
	try {
		return babelTransform(input, babelConfig).code;
	} catch (ex) {
		// Parsing stack is extremely long and not very useful, so just rethrow the message.
		throw new Error(ex.message);
	}
}

export function ReactNET_transform_sourcemap(input, babelConfig, filename) {
	babelConfig = {
		...JSON.parse(babelConfig),
		ast: false,
		filename,
		sourceMaps: true,
	};
	try {
		var result = babelTransform(input, babelConfig);
		return JSON.stringify({
			babelVersion,
			code: result.code,
			sourceMap: result.map
		});
	} catch (ex) {
		// Parsing stack is extremely long and not very useful, so just rethrow the message.
		throw new Error(ex.message);
	}
}