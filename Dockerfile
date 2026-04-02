FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY global.json ./
COPY QuantityMeasurementApp.sln ./
COPY ModelLayer/ModelLayer.csproj ModelLayer/
COPY RepositoryLayer/RepositoryLayer.csproj RepositoryLayer/
COPY ServiceLayer/ServiceLayer.csproj ServiceLayer/
COPY ControllerLayer/ControllerLayer.csproj ControllerLayer/

RUN dotnet restore ControllerLayer/ControllerLayer.csproj

COPY . .

RUN dotnet publish ControllerLayer/ControllerLayer.csproj -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app

COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:10000

EXPOSE 10000

ENTRYPOINT ["dotnet", "ControllerLayer.dll"]