#!/bin/bash

if [ $# -ne 1 ]; then
	echo "usage: $0 <destination>" >&2
	exit 1
fi

cp -r media $1/ &&
cp -r assets/font-awesome-4.7.0/fonts/. $1/ &&
cp assets/font-awesome-4.7.0/css/font-awesome.min.css $1/font-awesome.css &&
cp assets/prism/prism.{css,js} $1/ &&
cp assets/site.{css,js} $1/
