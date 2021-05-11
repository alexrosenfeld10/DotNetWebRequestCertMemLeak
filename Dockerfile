FROM mcr.microsoft.com/dotnet/aspnet:5.0-buster-slim

RUN apt-get update
RUN apt-get install -y wget procps lsof net-tools apt-transport-https curl jq && \
  wget https://packages.microsoft.com/config/debian/10/packages-microsoft-prod.deb -O packages-microsoft-prod.deb && \
  dpkg -i packages-microsoft-prod.deb
RUN apt-get update
RUN apt-get install -y dotnet-sdk-5.0 && \
    dotnet tool install --global dotnet-gcdump && \
    dotnet tool install --global dotnet-dump && \
    dotnet tool install --global dotnet-symbol && \
    dotnet tool install --global dotnet-sos

ENV CACHE_BUSTER=1

COPY DotNetNewRelicMemoryLeak.sln /src/DotNetNewRelicMemoryLeak.sln
COPY DotNetNewRelicMemoryLeak/DotNetNewRelicMemoryLeak.csproj /src/DotNetNewRelicMemoryLeak/DotNetNewRelicMemoryLeak.csproj

RUN dotnet restore /src/DotNetNewRelicMemoryLeak.sln

COPY DotNetNewRelicMemoryLeak /src/DotNetNewRelicMemoryLeak

# Create dotnet artifacts, output to /app
RUN dotnet publish --no-restore --configuration Release --output /app /src/DotNetNewRelicMemoryLeak.sln

WORKDIR /app
EXPOSE 5001
ENTRYPOINT ["dotnet", "/app/DotNetNewRelicMemoryLeak.dll"]
