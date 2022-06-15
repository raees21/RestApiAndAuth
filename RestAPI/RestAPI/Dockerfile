FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["RestAPI.csproj", "./"]
RUN dotnet restore "RestAPI.csproj"
COPY . .
WORKDIR "/src/"
RUN dotnet build "RestAPI.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "RestAPI.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "RestAPI.dll"]
