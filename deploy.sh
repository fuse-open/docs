#!/bin/bash
SELF=`echo $0 | sed 's/\\\\/\\//g'`
cd "`dirname "$SELF"`" || exit 1
set -e

#### Prerequisites
#
# 1. Make sure you have another clone of this repo called 'gh-pages',
#    located in this folder.
#
# 2. Make sure this clone has the 'gh-pages' branch checked out.
#
# 3. Make sure your working tree is clean and up-to-date.
#

SRC="_site"
DST="gh-pages"

if [ ! -d "$DST"/.git ]; then
    >&2 echo "fatal: Destination '$DST' is not a Git repository"
    exit 1
fi

# Read last commit SHA from source Git repository
SHA=`git rev-parse HEAD 2> /dev/null || :`

echo -e "Deploying $SHA\n"
bash build.sh
cp -R "$SRC"/* "$DST"

cd "$DST"
git add *
git commit -am"Deploy $SHA"
git status
git push

# Please inspect what you just pushed!
git show -1 || :
