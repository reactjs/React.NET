/*
 *  Copyright (c) 2015, Facebook, Inc.
 *  All rights reserved.
 *
 *  This source code is licensed under the BSD-style license found in the
 *  LICENSE file in the root directory of this source tree. An additional grant 
 *  of patent rights can be found in the PATENTS file in the same directory.
 */

var path = require('path');

module.exports = {
  context: path.join(__dirname, 'Content'),
  entry: {
    server: './server',
    client: './client'
  },
  output: {
    path: path.join(__dirname, 'build'),
    filename: '[name].bundle.js'
  },
  module: {
    loaders: [
      // Transform JavaScript files via Babel
      {
      	test: /\.jsx?$/,
      	exclude: /node_modules/,
      	loader: 'babel'
      }
	  // Uncomment this if you want to use your own version of React instead of the version 
	  // bundled with ReactJS.NET.
	  //{ test: require.resolve('react'), loader: 'expose?React' }
    ],
  },
  resolve: {
    // Allow require('./blah') to require blah.jsx
    extensions: ['', '.js', '.jsx']
  },
  externals: {
    // Use external version of React (from CDN for client-side, or bundled with ReactJS.NET for server-side)
	// Comment this out if you want to load your own version of React
    react: 'React'
  }
};