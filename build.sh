#!/bin/bash
SELF=`echo $0 | sed 's/\\\\/\\//g'`
cd "`dirname "$SELF"`" || exit 1
set -e

DST="_site"
URL="https://fuseopen.com/docs/"

CP_FLAGS=""
GEN_FLAGS=""
FAST=0

while [ $# -gt 0 ]; do
    case "$1" in
    -l|--local)
        shift
        URL="http://localhost:8080/"
        ;;
    -f|--fast)
        shift
        if [[ "$OSTYPE" = msys* ]]; then
            CP_FLAGS="--update"
        fi
        GEN_FLAGS="--fast"
        FAST=1
        ;;
    *)
        >&2 echo "Skipping unrecognized argument $1"
        shift
        ;;
    esac
done

echo -e "URL: $URL (Please wait for docs to finish building...)\n"

if [ "$BUILD" != 0 ]; then
    dotnet run --project generator/src/generator.csproj -- . "$URL" "$DST" $GEN_FLAGS
fi

if [ "$ASSETS" != 0 ]; then
    echo "Updating assets"
    cp -r $CP_FLAGS media "$DST"/ &&
    cp -r $CP_FLAGS static/* "$DST"/

    if [[ "$FAST" = 0 && "$OSTYPE" != msys* ]]; then
        echo "Optimizing images"
        find "$DST"/media -type f \( -iname "*.png" -or -iname "*.jpg" \) -exec mogrify -strip -resize 850x850\> {} \;
        find "$DST"/media -type f -iname "*.png" -exec pngquant {} \; -exec optipng -silent {} \;
    fi
fi

if [ ! -f "$DST"/.nojekyll ]; then
    touch "$DST"/.nojekyll
fi
