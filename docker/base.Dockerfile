# Dockerfile for the base image on which all ConferencePlanner services run
# NOTE: Build Context for this should be the parent directory

FROM microsoft/dotnet-nightly:2.0.0-preview3-runtime-deps

RUN mkdir /opt/app
WORKDIR /opt/app

RUN apt-get update \
    && apt-get install -y --no-install-recommends curl \
    && rm -rf /var/lib/apt/lists/*

# Copy in build scripts and set up dotnet cli
# Apparently it's not really possible to combine these copies into one layer :(
COPY ./scripts/_common.sh ./scripts/
COPY ./scripts/dotnet-install.sh ./scripts/link-dotnet.sh ./scripts/

COPY ./global.json .

ARG dotnet_sdk_version
RUN if [ -z "$dotnet_sdk_version" ]; then echo "dotnet_sdk_version build argument not set" 1>&2; exit 1; fi \
    && echo "Installing .NET SDK $dotnet_sdk_version" \
    && ./scripts/dotnet-install.sh --install-dir /opt/dotnet --runtime-id linux-x64 --version $dotnet_sdk_version \
    && rm -rf /opt/dotnet/sdk \
    && ./scripts/link-dotnet.sh \
    && rm -Rf ./global.json ./scripts

LABEL dotnet.sdk.version=$dotnet_sdk_version