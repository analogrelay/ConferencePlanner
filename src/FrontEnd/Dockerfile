# The version of docker on VSTS doesn't support ARG coming before FROM :(
# So this will need to be manually updated when the SDK version is updated
FROM anurse/conferenceplanner-base:sdk.2.0.0-preview3-006845

ENV ASPNETCORE_URLS http://0.0.0.0:5101

WORKDIR /opt/app

COPY ./ ./

ENTRYPOINT [ "/opt/dotnet/dotnet", "/opt/app/FrontEnd.dll" ]
