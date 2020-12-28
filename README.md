# Fuse Documentation

[![Build Status](https://travis-ci.com/fuse-open/docs.svg?branch=master)](https://travis-ci.com/fuse-open/docs)

This is the source code for the documentation for Fuse, as hosted [here](https://fuse-open.github.io/docs).

The documentation is hosted on GitHub Pages, from the [gh-pages branch](https://github.com/fuse-open/docs/tree/gh-pages). This branch gets automatically updated by a Travis CI build-step.

## Running locally

1. Run `dotnet run -p generator/src/generator.csproj -- . "http://localhost:8000/" _site` to build the HTML files.
2. Run `./copy-assets.bash _site` to copy assets into the target directory.
3. Run `find _site/media -type f \( -iname "*.png" -or -iname "*.jpg" \) -exec mogrify -strip -resize 450x450\> {} \;` to resize images to the appropriate sizes.
4. Run `python3 -m http.server 8000 --directory _site/` (or whatever your favorite static http server is) to serve the website at port 8000.
5. Open `http://localhost/:8000` in your web browser to view the result.

## Updating `api-docs`

1. Run `cd doc-export` to enter the project used to make doc-files.
2. Run `npm install` to install dependencies.
3. *Optional:* Run `npm install -S fuse-sdk` to upgrade to the latest version of Fuse SDK.
4. Run `npm run build` to update `api-docs`.
