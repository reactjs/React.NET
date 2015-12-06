/*
 *  Copyright (c) 2015, Facebook, Inc.
 *  All rights reserved.
 *
 *  This source code is licensed under the BSD-style license found in the
 *  LICENSE file in the root directory of this source tree. An additional grant 
 *  of patent rights can be found in the PATENTS file in the same directory.
 */

var gulp = require('gulp');
var named = require('vinyl-named');
var webpack = require('webpack');
var webpackStream = require('webpack-stream');
var uglify = require('gulp-uglify');

var OUTPUT_DIR = 'Resources/';

gulp.task('default', ['build']);
gulp.task('build', ['build-react-dev', 'build-deps-prod']);

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
				}),
				new webpack.optimize.OccurenceOrderPlugin(),
				new webpack.optimize.DedupePlugin()
			]
		}))
		.pipe(gulp.dest(OUTPUT_DIR));
});

gulp.task('build-deps-prod', function () {
	return gulp.src(['Resources/react.js', 'Resources/babel.js'])
		.pipe(named())
		.pipe(webpackStream({
			module: {
				loaders: [
					{
						exclude: /node_modules/,
						test: /\.js$/,
						loader: 'babel',
						query: {
							presets: ['es2015', 'stage-0']
						}
					},
				]
			},
			output: {
				filename: '[name].generated.min.js',
				libraryTarget: 'this'
			},
			plugins: [
				new webpack.DefinePlugin({
					'process.env.NODE_ENV': '"production"'
				}),
				new webpack.optimize.OccurenceOrderPlugin(),
				new webpack.optimize.DedupePlugin()
			]
		}))
		.pipe(uglify())
		.pipe(gulp.dest(OUTPUT_DIR));
});
