#!/usr/bin/env bash
DIR="$( cd "$( dirname "${BASH_SOURCE[0]}" )" && pwd )"

has() {
    local CMD=$1
    type -p $CMD >/dev/null 2>/dev/null
}

die() {
    echo $1
    exit 1
}

# Ensure dotnet is on the PATH
[ ! -z $DOTNET_INSTALL_DIR ] || DOTNET_INSTALL_DIR="$HOME/.dotnet"
export PATH="$DOTNET_INSTALL_DIR:$PATH"

# Determine the expected cli version
sdk_channel=$(cat global.json | grep "\"channel\":" | sed 's/^ *"channel": "\([^"]*\)",*/\1/')
sdk_version=$(cat global.json | grep "\"version\":" | sed 's/^ *"version": "\([^"]*\)",*/\1/')

if ! has dotnet || [ "$(dotnet --version)" != "$sdk_version" ]; then
    # Install the right version of the CLI
    "$DIR/build/dotnet-install.sh" --channel $sdk_channel --version $sdk_version --install-dir $DOTNET_INSTALL_DIR
fi

# Disable first time experience (it's for devs, not automated builds)
export DOTNET_SKIP_FIRST_TIME_EXPERIENCE=true

echo "Cleaning bin/obj"

echo "Restoring Packages"
dotnet restore

echo "Building"
dotnet build

artifacts_path="$DIR/artifacts/linux"
for proj in FrontEnd BackEnd; do
    project="$DIR/src/$proj/$proj.csproj"
    output="$artifacts_path/$proj"
    echo "Publishing $proj"
    dotnet publish $project -o $output
done
