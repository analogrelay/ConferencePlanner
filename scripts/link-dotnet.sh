#!/usr/bin/env bash
DIR="$( cd "$( dirname "${BASH_SOURCE[0]}" )" && pwd )"
REPOROOT="$( cd "$DIR/.." && pwd )"

source "$REPOROOT/scripts/_common.sh"

if [ -e /usr/bin/dotnet ]; then
    rm /usr/bin/dotnet
fi

ln -s /opt/dotnet/dotnet /usr/bin/dotnet