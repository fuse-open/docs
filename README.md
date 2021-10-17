# Fuse Documentation

[![Build Status](https://travis-ci.com/fuse-open/docs.svg?branch=master)](https://travis-ci.com/fuse-open/docs)

This is the source code for the documentation for Fuse, as hosted [here](https://fuseopen.com/docs).

The documentation is hosted on GitHub Pages, from the [gh-pages branch](https://github.com/fuse-open/docs/tree/gh-pages). This branch gets automatically updated by a Travis CI build-step.

## Running locally

```shell
npm install
npm start
```

This will build HTML pages based on documentation files and start a webserver at http://localhost:8080/.

Some system dependencies are required to process files. On macOS they can be installed by running the following command.

```shell
brew install imagemagick pngquant optipng
```

> On Windows image processing is disabled so there we don't need to install the dependencies, but [Git for Windows](https://github.com/git-for-windows/git/releases) is nice to have for running scripts.

## Updating `api-docs`

1. Run `cd doc-export` to enter the project used to make doc-files.
2. Run `npm install` to install dependencies.
3. *Optional:* Run `npm install -S fuse-sdk` to upgrade to the latest version of Fuse SDK.
4. Run `npm run build` to update `api-docs`.
