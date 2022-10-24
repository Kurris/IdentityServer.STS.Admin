FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["src/QrCodeServer/QrCodeServer.csproj", "QrCodeServer/"]
RUN dotnet restore "src/QrCodeServer/QrCodeServer.csproj"
COPY . .
WORKDIR "/src/QrCodeServer"
RUN dotnet build "QrCodeServer.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "QrCodeServer.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "QrCodeServer.dll"]
