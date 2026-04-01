FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY src/*.slnx .
COPY src/PromoCodeFactory.Core/*.csproj PromoCodeFactory.Core/
COPY src/PromoCodeFactory.UnitTests/*.csproj PromoCodeFactory.UnitTests/
COPY src/PromoCodeFactory.DataAccess/*.csproj PromoCodeFactory.DataAccess/
COPY src/PromoCodeFactory.WebHost/*.csproj PromoCodeFactory.WebHost/
RUN dotnet restore

COPY src/ .
RUN dotnet build -c Release --no-restore
RUN dotnet publish -c Release -o /app/publish --no-build

FROM base AS final
COPY --from=build /app/publish .
EXPOSE 5000

ENTRYPOINT ["dotnet", "PromoCodeFactory.WebHost.dll"]

