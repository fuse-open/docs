#!/bin/bash

if [ $# -ne 1 ]; then
	echo "usage: $0 <destination>" >&2
	exit 1
fi

cp -r media $1/ &&
cp -r assets/font-awesome-4.7.0/fonts/. $1/ &&
cp assets/font-awesome-4.7.0/css/font-awesome.min.css $1/font-awesome.css &&
cp assets/bootstrap-4.0.0/css/bootstrap.min.css $1/bootstrap.css &&
cp assets/jquery-3.3.1/jquery-3.3.1.min.js $1/jquery.js &&
cp assets/popper.js-1.14.1/dist/umd/popper.min.js $1/popper.js &&
cp assets/bootstrap-4.0.0/js/bootstrap.min.js $1/bootstrap.js &&
cp assets/prism/prism.{css,js} $1/ &&
cp assets/site.{css,js} $1/
