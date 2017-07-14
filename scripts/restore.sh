#!/usr/bin/env bash
DIR="$( cd "$( dirname "${BASH_SOURCE[0]}" )" && pwd )"
REPOROOT="$( cd "$DIR/.." && pwd )"

source "$REPOROOT/scripts/_common.sh"

echo "Restoring Packages"
cd "$DIR/.."
dotnet restore