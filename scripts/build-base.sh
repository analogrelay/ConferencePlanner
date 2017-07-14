#!/usr/bin/env bash
DIR="$( cd "$( dirname "${BASH_SOURCE[0]}" )" && pwd )"
REPOROOT="$( cd "$DIR/.." && pwd )"

while [ $# -ne 0 ]; do
    name=$1
    case $name in
        --push)
            push=1
            ;;
    esac
    shift
done

source "$REPOROOT/scripts/_common.sh"

cd $REPOROOT

docker build \
    --file ./docker/base.Dockerfile \
    --tag "$docker_repo:base.sdk.$sdk_version" \
    --build-arg dotnet_sdk_version=$sdk_version \
    .

if [ $push = 1 ]; then
    docker push "$docker_repo:base.sdk.$sdk_version"
fi