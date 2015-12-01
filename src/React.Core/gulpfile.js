/*
 *  Copyright (c) 2015, Facebook, Inc.
 *  All rights reserved.
 *
 *  This source code is licensed under the BSD-style license found in the
 *  LICENSE file in the root directory of this source tree. An additional grant 
 *  of patent rights can be found in the PATENTS file in the same directory.
 */

var gulp = require('gulp');
var webpack = require('webpack');
var webpackStream = require('webpack-stream');
var uglify = require('gulp-uglify');

gulp.task('default', ['build']);
gulp.task('build', ['build-react-dev', 'build-react-prod']);

gulp.task('build-react-dev', function() {
	return gulp.src('Resources/react.js')
		.pipe(webpackStream({
			output: {
				filename: 'react.generated.js',
				libraryTarget: 'this'
			},
			plugins: [
			  new webpack.DefinePlugin({
			  	'process.env.NODE_ENV': '"development"'
			  })
			]
		}))
		.pipe(gulp.dest('Resources/'));
});

gulp.task('build-react-prod', function () {
	return gulp.src('Resources/react.js')
		.pipe(webpackStream({
			output: {
				filename: 'react.generated.min.js',
				libraryTarget: 'this'
			},
			plugins: [
			  new webpack.DefinePlugin({
			  	'process.env.NODE_ENV': '"production"'
			  })
			]
		}))
		.pipe(uglify())
		.pipe(gulp.dest('Resources/'));
});