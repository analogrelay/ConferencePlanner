#!/usr/bin/env bash
DIR="$( cd "$( dirname "${BASH_SOURCE[0]}" )" && pwd )"
REPOROOT="$( cd "$DIR/.." && pwd )"

source "$REPOROOT/scripts/_common.sh"

cd "$DIR/.."
echo "Building"
dotnet build

artifacts_path="$REPOROOT/artifacts/linux"
for proj in FrontEnd BackEnd; do
    project="$REPOROOT/src/$proj/$proj.csproj"
    output="$artifacts_path/$proj"
    echo "Publishing $proj"

    if [ -z $CONFERENCEPLANNER_BUILD_RID ]; then
        dotnet publish $project -o $output
    else
        dotnet publish $project -o $output --runtime $CONFERENCEPLANNER_BUILD_RID
    fi
done