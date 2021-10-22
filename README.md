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

### Fast Mode™

When building locally using `npm start`, Fast Mode™ is enabled by
default for the ability to test and iterate quickly. 🚀

In Fast Mode™ we skip processing `api-docs/`, images, and take a few
shortcuts -- so the resulting HTML pages are not complete, missing
Uno/UX reference pages and related links. Most things are still fine.

If you need to test a complete build, run this command instead.

```shell
npm run start-slow
```

### Other commands

Type this to see all commands that are available.

```shell
npm run
```

## Updating API docs

```shell
npm run api-docs
```

1. Run `npm install` to install dependencies.
2. *Optional:* Run `npm install -D fuse-sdk` to upgrade to latest versions of Fuse SDK modules.
    * Or, if you wish specific versions you can do that. Just install the versions of `@fuse-open/fuselibs` and/or `@fuse-open/uno` that you want.
    * Or, install a custom artifact, e.g. `https://ci.appveyor.com/api/buildjobs/wqgf7r6oiiwb16ff/artifacts/fuse-open-fuselibs-2.0.0.tgz`. You can find build artifacts on our AppVeyor pages for Uno and Fuselibs.
    * Or, install another module for adding to our API references.
    * Or, use `npm link` for any local modules you're working on.
    * Run `npm outdated` to see if anything is out of date.
3. Run `npm run api-docs` to update files in `api-docs/`, based on modules installed in step 2.
4. Run `npm run start-slow` to build new HTML pages, presented at http://localhost:8080/.
5. *Optional:* Open a [Pull Request](https://github.com/fuse-open/docs/pulls) if you did something good. :)
