/*
 *  Copyright (c) 2014, Facebook, Inc.
 *  All rights reserved.
 *
 *  This source code is licensed under the BSD-style license found in the
 *  LICENSE file in the root directory of this source tree. An additional grant 
 *  of patent rights can be found in the PATENTS file in the same directory.
 */

var global = global || {};
// TODO: Handle errors "thrown" by console.error / console.warn?
var console = console || {
	log: function () { },
	error: function () { },
	warn: function () { }
};

if (!Object.freeze) {
	Object.freeze = function() { };
}

function ReactNET_transform(input, harmony, stripTypes) {
	try {
		return global.JSXTransformer.transform(input, {
			harmony: !!harmony,
			stripTypes: !!stripTypes
		}).code;
	} catch (ex) {
		throw new Error(ex.message + " (at line " + ex.lineNumber + " column " + ex.column + ")");
	}
}

function ReactNET_transform_sourcemap(input, harmony, stripTypes) {
	try {
		var result = global.JSXTransformer.transform(input, {
			harmony: !!harmony,
			stripTypes: !!stripTypes,
			sourceMap: true
		});
		if (!result.sourceMap) {
			return JSON.stringify({
				code: result.code,
				sourceMap: null
			});
		}

		return JSON.stringify({
			code: result.code,
			sourceMap: result.sourceMap.toJSON()
		});
	} catch (ex) {
		throw new Error(ex.message + " (at line " + ex.lineNumber + " column " + ex.column + ")");
	}
}