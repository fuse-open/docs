#!/bin/bash
set -e
dotnet run -p generator/src/generator.csproj -- . "https://fuseopen.com/docs/" _site
./copy-assets.bash _site
find _site/media -type f \( -iname "*.png" -or -iname "*.jpg" \) -exec mogrify -strip -resize 850x850\> {} \;
find _site/media -type f -iname "*.png" -exec pngquant {} \; -exec optipng -silent {} \;
touch _site/.nojekyll