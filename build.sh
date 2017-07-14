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

while [ $# -ne 0 ]; do
    name=$1
    case $name in
        --docker)
            build_in_docker=1
            ;;
        *)
            break
            ;;
    esac
    shift
done

if [ $build_in_docker = 1 ]; then
    "$DIR/scripts/docker-build.sh" "$@"
else
    "$DIR/scripts/install-dotnet.sh"
    "$DIR/scripts/restore.sh"
    "$DIR/scripts/compile.sh"
fi