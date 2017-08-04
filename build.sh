#!/usr/bin/env bash
DIR="$( cd "$( dirname "${BASH_SOURCE[0]}" )" && pwd )"
REPOROOT="$( cd "$DIR/.." && pwd )"

source "$REPOROOT/scripts/_common.sh"

cd "$REPOROOT"

echo "Verifying dotnet CLI version"
if ! has dotnet || [ "$(dotnet --version)" != "$sdk_version" ]; then
    # Install the right version of the CLI
    echo "Installing dotnet CLI $sdk_version ..."

    # Because we only have an SDK version to pivot on, we have to get the whole SDK even if only the runtime was requested
    "$DIR/dotnet-install.sh" --version $sdk_version --install-dir $DOTNET_INSTALL_DIR --runtime-id linux-x64
fi

echo "Restoring Packages"
dotnet restore "ConferencePlanner.sln"

echo "Building"
dotnet publish "ConferencePlanner.sln"

echo "Build succeeded"