#!/usr/bin/env bash
DIR="$( cd "$( dirname "${BASH_SOURCE[0]}" )" && pwd )"
REPOROOT="$( cd "$DIR/.." && pwd )"

source "$REPOROOT/scripts/_common.sh"

# Just to protect against updating global.json without updating Dockerfiles
# Because VSTS's docker doesn't support ARG before FROM yet :(
if [ $sdk_version != "2.0.0-preview3-006701" ]; then
    echo "error: The global.json SDK version has been updated, but the docker-build.sh file, and the Dockerfiles in the project repos may not have been." 1>&2
    exit 1
fi

cd $REPOROOT

base_image_tag="base.sdk.${sdk_version}"

docker build \
    --tag $docker_repo:build \
    --file ./docker/build.Dockerfile \
    .

[ -d "$PWD/artifacts" ] || mkdir "$PWD/artifacts"

docker run \
    --interactive \
    --volume "$PWD/artifacts:/opt/code/artifacts" \
    $docker_repo:build

# Get commit hash
commit_hash=$(git rev-parse HEAD)

if [ -z "$BUILD_SOURCEBRANCH" ]; then
    commit_branch=$(git rev-parse --abbrev-ref HEAD)
else
    # VSTS gave us a source branch, but in the form 'refs/heads/...'
    # We could also get the "last" segment of the branch name in a different variable, but
    # that means that branches like "anurse/foo" would just become "foo", which we don't want
    # So we manually parse the full name down here
    commit_branch=${BUILD_SOURCEBRANCH#refs/heads/}
fi

# Build each of the output containers
built_containers=()
for dockerfile in $(find "$REPOROOT/artifacts/linux" -name Dockerfile); do
    context_dir=$(dirname $dockerfile)
    name=$(basename $context_dir)
    image_branch="$docker_repo:service.$name.${commit_branch//\//-}"
    image_hash="$docker_repo:service.$name.$commit_hash"
    echo "Building Docker Image $image_hash"
    docker build \
        --build-arg BASE_IMAGE_TAG=$base_image_tag \
        --tag $image_hash \
        --tag $image_branch \
        --label commit.hash=$commit_hash \
        --label commit.branch=$commit_branch \
        --file $dockerfile \
        $context_dir

    containers+=("$image_hash")
done

echo "Docker containers built:"
for built_container in ${built_containers[@]}; do
    echo "* $built_container"
done