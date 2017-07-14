#!/usr/bin/env bash
DIR="$( cd "$( dirname "${BASH_SOURCE[0]}" )" && pwd )"
REPOROOT="$( cd "$DIR/.." && pwd )"

source "$REPOROOT/scripts/_common.sh"

cd $REPOROOT

docker build \
    --tag $docker_repo:build \
    --file ./docker/build.Dockerfile \
    .

[ -d "$PWD/artifacts" ] || mkdir "$PWD/artifacts"

docker run \
    --tty \
    --interactive \
    --volume "$PWD/artifacts:/opt/code/artifacts" \
    $docker_repo:build

base_image_tag="base.sdk.${sdk_version}"

# Get commit hash
commit_hash=$(git rev-parse HEAD)
commit_branch=$(git rev-parse --abbrev-ref HEAD)

# Build each of the output containers
containers=()
for dockerfile in $(find "$REPOROOT/artifacts/linux" -name Dockerfile); do
    context_dir=$(dirname $dockerfile)
    name=$(basename $context_dir)
    image_branch="$docker_repo:service.$name.${commit_branch//\//-}"
    image_hash="$docker_repo:service.$name.$commit_hash"
    echo "Building Docker Image $image_hash"
    docker build \
        --build-arg BASE_IMAGE_TAG=$base_image_tag \
        --tag $image_hash \
        --file $dockerfile \
        $context_dir

    echo "Applying tag $image_branch"
    docker tag \
        $image_hash \
        $image_branch

    containers+=("$image_branch" "$image_hash")
done

echo "Docker containers built!"
for container in $containers; do
    echo "* $container"
done