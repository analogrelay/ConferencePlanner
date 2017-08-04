# NOTE: This script is copied to docker containers very early in the build
# Adding something here will invalidate most of the Docker build cache. That's OK, but it may slow down some builds

# Fail when any command fails
set -e

has() {
    local CMD=$1
    type -p $CMD >/dev/null 2>/dev/null
}

die() {
    echo $1 1>&2
    exit 1
}

if [ -z "$REPOROOT" ]; then
    die "REPOROOT must be set before sourcing _common.sh"
fi

# Determine the cli version
sdk_version=$(cat "$REPOROOT/global.json" | grep "\"version\":" | sed 's/^ *"version": "\([^"]*\)",*/\1/')
docker_repo=anurseconferenceplanner.azurecr.io
base_docker_repo=anurse/conferenceplanner-base