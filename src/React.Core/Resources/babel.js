/*
 *  Copyright (c) 2015, Facebook, Inc.
 *  All rights reserved.
 *
 *  This source code is licensed under the BSD-style license found in the
 *  LICENSE file in the root directory of this source tree. An additional grant 
 *  of patent rights can be found in the PATENTS file in the same directory.
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