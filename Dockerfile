FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY . .
RUN dotnet publish src/Branch.Platform.Api/Branch.Platform.Api.csproj -c Release -o /app/api
RUN dotnet publish src/Branch.Platform.Worker/Branch.Platform.Worker.csproj -c Release -o /app/worker

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/api ./
COPY --from=build /app/worker ./
ENTRYPOINT ["dotnet", "Branch.Platform.Api.dll"]
