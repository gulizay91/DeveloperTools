FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["DeveloperTools.RabbitMQHelper/DeveloperTools.RabbitMQHelper.csproj", "DeveloperTools.RabbitMQHelper/"]
RUN dotnet restore "DeveloperTools.RabbitMQHelper/DeveloperTools.RabbitMQHelper.csproj"
COPY . .
WORKDIR "/src/DeveloperTools.RabbitMQHelper"
RUN dotnet build "DeveloperTools.RabbitMQHelper.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "DeveloperTools.RabbitMQHelper.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DeveloperTools.RabbitMQHelper.dll"]
