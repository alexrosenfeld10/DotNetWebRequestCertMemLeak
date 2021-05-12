FROM mcr.microsoft.com/dotnet/sdk:5.0

COPY DotNetWebRequestCertMemLeak.sln /src/DotNetWebRequestCertMemLeak.sln
COPY DotNetWebRequestCertMemLeak/DotNetWebRequestCertMemLeak.csproj /src/DotNetWebRequestCertMemLeak/DotNetWebRequestCertMemLeak.csproj

RUN dotnet restore /src/DotNetWebRequestCertMemLeak.sln

COPY DotNetWebRequestCertMemLeak /src/DotNetWebRequestCertMemLeak

# Create dotnet artifacts, output to /app
RUN dotnet publish --no-restore --configuration Release --output /app /src/DotNetWebRequestCertMemLeak.sln

WORKDIR /app
ENTRYPOINT ["dotnet", "/app/DotNetWebRequestCertMemLeak.dll"]
