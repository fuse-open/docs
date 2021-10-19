# Fuse Documentation

[![Build Status](https://travis-ci.com/fuse-open/docs.svg?branch=master)](https://travis-ci.com/fuse-open/docs)
[![PRs Welcome](https://img.shields.io/badge/PRs-welcome-brightgreen.svg)](https://github.com/fuse-open/docs/pulls)

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

> For more details about build commands see [build.sh](build.sh).

## Updating API docs

```shell
npm run api-docs
```

1. Run `npm install` to install dependencies.
2. *Optional:* Run `npm install -D fuse-sdk` to upgrade to the latest version of Fuse SDK.
    * Or, if you wish you update API docs from another module you can do that, e.g. `@fuse-open/fuselibs` or `@fuse-open/uno`.
    * Or, from a custom artifact, e.g. `https://ci.appveyor.com/api/buildjobs/wqgf7r6oiiwb16ff/artifacts/fuse-open-fuselibs-2.0.0.tgz`.
3. Run `npm run api-docs` to update files in `api-docs/`, based on modules installed in step 2.
4. Run `npm start` to build new HTML pages, presented at http://localhost:8080/.
5. *Optional:* Open a [Pull Request](https://github.com/fuse-open/docs/pulls) if you did something good. :)
