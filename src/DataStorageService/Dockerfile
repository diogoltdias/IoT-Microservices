FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
LABEL service.name="datastorageservice"

WORKDIR /app
COPY Common/ ./Common/
COPY DataStorageService/ ./DataStorageService/

WORKDIR /app/DataStorageService
RUN dotnet restore --source https://api.nuget.org/v3/index.json
RUN dotnet build -c Release -o /app/build
RUN dotnet publish -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "DataStorageService.dll"]