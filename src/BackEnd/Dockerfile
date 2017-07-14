ARG BASE_IMAGE_TAG=base
FROM anurse/conferenceplanner:${BASE_IMAGE_TAG}

# In Docker, all apps run on 0.0.0.0:5000
ENV ASPNETCORE_URLS http://0.0.0.0:5000

WORKDIR /opt/app

COPY ./ ./

ENTRYPOINT [ "/opt/dotnet/dotnet", "/opt/app/BackEnd.dll" ]
