FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["EuroStocks.Consumers/EuroStocks.Consumers.csproj", "EuroStocks.Consumers/"]
RUN dotnet restore "EuroStocks.Consumers/EuroStocks.Consumers.csproj"
COPY . .
WORKDIR "/src/EuroStocks.Consumers"
RUN dotnet build "EuroStocks.Consumers.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "EuroStocks.Consumers.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "EuroStocks.Consumers.dll"]
