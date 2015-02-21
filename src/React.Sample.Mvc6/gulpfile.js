/*
 *  Copyright (c) 2015, Facebook, Inc.
 *  All rights reserved.
 *
 *  This source code is licensed under the BSD-style license found in the
 *  LICENSE file in the root directory of this source tree. An additional grant 
 *  of patent rights can be found in the PATENTS file in the same directory.
 */
'use strict';

var gulp = require('gulp');
var react = require('gulp-react');

gulp.task('default', function() {
    gulp.src('wwwroot/js/Sample.jsx')
        .pipe(react({harmony: true}))
        .pipe(gulp.dest('wwwroot/js'));
});