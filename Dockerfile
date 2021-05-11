FROM mcr.microsoft.com/dotnet/aspnet:5.0-buster-slim

RUN apt-get update
RUN apt-get install -y wget procps lsof net-tools apt-transport-https && \
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
COPY DotNetNewRelicMemoryLeak /src/DotNetNewRelicMemoryLeak

RUN dotnet restore /src/DotNetNewRelicMemoryLeak.sln

# Create dotnet artifacts, output to /app
RUN dotnet publish --no-restore --configuration Release --output /app /src/DotNetNewRelicMemoryLeak.sln

ENV CORECLR_ENABLE_PROFILING=1 \
    CORECLR_PROFILER={36032161-FFC0-4B61-B559-F6C5D41BAE5A} \
    CORECLR_NEWRELIC_HOME="/app/newrelic" \
    CORECLR_PROFILER_PATH="/app/newrelic/libNewRelicProfiler.so" \
    NEW_RELIC_DISTRIBUTED_TRACING_ENABLED=true \
    NEW_RELIC_APP_NAME="DotNetNewRelicMemoryLeak" \
    NEW_RELIC_LICENSE_KEY="YOUR KEY HERE"

WORKDIR /app
EXPOSE 5001
ENTRYPOINT ["dotnet", "/app/DotNetNewRelicMemoryLeak.dll"]
