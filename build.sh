#!/usr/bin/env bash
REPOROOT="$( cd "$( dirname "${BASH_SOURCE[0]}" )" && pwd )"

[ ! -z "$BuildConfiguration" ] || BuildConfiguration="Debug"

export DOTNET_SKIP_FIRST_TIME_EXPERIENCE=1

source "$REPOROOT/scripts/_common.sh"

cd "$REPOROOT"

echo "Running $BuildConfiguration Build..."

echo "Verifying dotnet CLI version"
if ! has dotnet || [ "$(dotnet --version)" != "$sdk_version" ]; then
    # Install the right version of the CLI
    echo "Installing dotnet CLI $sdk_version ..."

    # Because we only have an SDK version to pivot on, we have to get the whole SDK even if only the runtime was requested
    "$REPOROOT/scripts/dotnet-install.sh" --version $sdk_version --runtime-id linux-x64
fi

echo "Restoring Packages"
dotnet restore "ConferencePlanner.sln"

echo "Building"
dotnet publish "ConferencePlanner.sln" --configuration $BuildConfiguration

echo "Build succeeded"