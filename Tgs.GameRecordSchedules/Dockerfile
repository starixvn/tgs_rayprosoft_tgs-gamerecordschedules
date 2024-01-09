#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081
EXPOSE 9090

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["nuget.config", "."]
COPY ["Tgs.GameRecordSchedules/Tgs.GameRecordSchedules.csproj", "Tgs.GameRecordSchedules/"]
COPY ["Tgs.GameRecordSchedules.Core/Tgs.GameRecordSchedules.Core.csproj", "Tgs.GameRecordSchedules.Core/"]
COPY ["Tgs.GameRecordSchedules.Repositories/Tgs.GameRecordSchedules.Repositories.csproj", "Tgs.GameRecordSchedules.Repositories/"]
RUN dotnet restore "./Tgs.GameRecordSchedules/./Tgs.GameRecordSchedules.csproj"
COPY . .
WORKDIR "/src/Tgs.GameRecordSchedules"
RUN dotnet build "./Tgs.GameRecordSchedules.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Tgs.GameRecordSchedules.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Tgs.GameRecordSchedules.dll"]