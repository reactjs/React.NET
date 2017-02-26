#!/bin/bash
set -ex
cd jekyll
bundle install
bundle exec jekyll build
cd ..
