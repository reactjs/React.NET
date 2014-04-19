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

function ReactNET_transform(input) {
	return global.JSXTransformer.transform(input, { harmony: true }).code;
}