#!/bin/bash
SELF=`echo $0 | sed 's/\\\\/\\//g'`
cd "`dirname "$SELF"`" || exit 1
set -e

DST="_site"
URL="https://fuseopen.com/docs/"

while [ $# -gt 0 ]; do
    case "$1" in
    -l|--local)
        shift
        URL="http://localhost:8080/"
        ;;
    *)
        break
        ;;
    esac
done

echo "URL: $URL (Please wait for docs to finish building...)"

if [ "$BUILD" != 0 ]; then
    dotnet run -p generator/src/generator.csproj -- . "$URL" "$DST"
fi

if [ "$ASSETS" != 0 ]; then
    cp -r media "$DST"/ &&
    cp -r static/* "$DST"/

    if [[ "$OSTYPE" != msys* ]]; then
        find _site/media -type f \( -iname "*.png" -or -iname "*.jpg" \) -exec mogrify -strip -resize 850x850\> {} \;
        find _site/media -type f -iname "*.png" -exec pngquant {} \; -exec optipng -silent {} \;
    fi
fi

if [ ! -f _site/.nojekyll ]; then
    touch _site/.nojekyll
fi
