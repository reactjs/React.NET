﻿/*
 *  Copyright (c) 2014-Present, Facebook, Inc.
 *  All rights reserved.
 *
 *  This source code is licensed under the BSD-style license found in the
 *  LICENSE file in the root directory of this source tree. An additional grant 
 *  of patent rights can be found in the PATENTS file in the same directory.
 */

var global = global || {};
var React, ReactDOM, ReactDOMServer;

// Basic console shim. Caches all calls to console methods.
function MockConsole() {
	this._calls = [];
	['log', 'error', 'warn', 'debug', 'info', 'dir', 'group', 'groupEnd', 'groupCollapsed'].forEach(function (methodName) {
		this[methodName] = this._handleCall.bind(this, methodName);
	}, this);
}
MockConsole.prototype = {
	_handleCall: function(methodName/*, ...args*/) {
		var serializedArgs = [];
		for (var i = 1; i < arguments.length; i++) {
			serializedArgs.push(JSON.stringify(arguments[i]));
		}

		this._calls.push({
			method: methodName,
			args: serializedArgs,
			stack: '\nCall stack: ' + (new Error().stack || 'not available')
		});
	},
	_formatCall: function(call) {
		return 'console.' + call.method + '("[.NET]", ' + call.args.join(', ') + ', ' + JSON.stringify(call.stack) + ');';
	},
	getCalls: function() {
		return this._calls.map(this._formatCall).join('\n');
	}
};
var console = new MockConsole();

if (!Object.freeze) {
	Object.freeze = function() { };
}

/**
 * Finds a user-supplied version of React and ensures it's exposed globally.
 *
 * @return {bool}
 */
function ReactNET_initReact() {
	if (
		typeof React !== 'undefined' &&
		typeof ReactDOM !== 'undefined' &&
		typeof ReactDOMServer !== 'undefined'
	) {
		// React is already a global, woohoo
		return true;
	}

	if (global.React && global.ReactDOM && global.ReactDOMServer) {
		React = global.React;
		ReactDOM = global.ReactDOM;
		ReactDOMServer = global.ReactDOMServer;
		return true;
	}
	// :'(
	return false;
}

/**
 * Polyfill for engines that do not support Object.assign
 */
if (typeof Object.assign !== 'function') {
	Object.assign = function (target, varArgs) { // .length of function is 2
		'use strict';
		if (target == null) { // TypeError if undefined or null
			throw new TypeError('Cannot convert undefined or null to object');
		}

		var to = Object(target);

		for (var index = 1; index < arguments.length; index++) {
			var nextSource = arguments[index];

			if (nextSource != null) { // Skip over if undefined or null
				for (var nextKey in nextSource) {
					// Avoid bugs when hasOwnProperty is shadowed
					if (Object.prototype.hasOwnProperty.call(nextSource, nextKey)) {
						to[nextKey] = nextSource[nextKey];
					}
				}
			}
		}
		return to;
	};
}