# Dockerfile for building the image in which the ConferencePlanner app is built
# NOTE: Build Context for this should be the parent directory

FROM microsoft/dotnet-nightly:2.0.0-preview3-runtime-deps

ENV DOTNET_INSTALL_DIR /opt/dotnet

RUN mkdir /opt/code
WORKDIR /opt/code

RUN apt-get update \
    && apt-get install -y --no-install-recommends curl \
    && rm -rf /var/lib/apt/lists/*

# Copy in build scripts and set up dotnet cli
# Apparently it's not really possible to combine these copies into one layer :(
COPY ./scripts/_common.sh ./scripts/
COPY ./scripts/install-dotnet.sh ./scripts/dotnet-install.sh ./scripts/link-dotnet.sh ./scripts/

COPY ./global.json .
RUN ./scripts/install-dotnet.sh && ./scripts/link-dotnet.sh

# Run the population of the local package cache in a layer
RUN mkdir warmup \
    && cd warmup \
    && dotnet new web \
    && cd .. \
    && rm -rf warmup \
    && rm -rf /tmp/NuGetScratch

# Copy the projects in and restore them to create a layer with all the nuget packages restored.
# No way to do this all in one layer :(. Could maybe use multi-stage build, create a 'FROM scratch' container
# and then mount it during the 'RUN ./scripts/restore.sh' to avoid building up layers here?
COPY ./ConferencePlanner.sln .
COPY ./src/FrontEnd/FrontEnd.csproj ./src/FrontEnd/FrontEnd.csproj
COPY ./src/BackEnd/BackEnd.csproj ./src/BackEnd/BackEnd.csproj
COPY ./src/ConferenceDTO/ConferenceDTO.csproj ./src/ConferenceDTO/ConferenceDTO.csproj
COPY ./src/Directory.Build.props ./src/Directory.Build.props
COPY ./build/ ./build/
COPY ./NuGet.config ./NuGet.config
COPY ./scripts/restore.sh ./scripts/
RUN ./scripts/restore.sh

# Copy in the rest of the sources
COPY ./ ./

ENTRYPOINT ./scripts/compile.sh
