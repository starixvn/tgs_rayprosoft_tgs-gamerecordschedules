#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
ARG BuildConfiguration=Release
WORKDIR /src
COPY ["nuget.config", "."]
COPY ["Tgs.GameRecordSchedules/Tgs.GameRecordSchedules.csproj", "Tgs.GameRecordSchedules/"]
COPY ["Tgs.GameRecordSchedules.Core/Tgs.GameRecordSchedules.Core.csproj", "Tgs.GameRecordSchedules.Core/"]
COPY ["Tgs.GameRecordSchedules.Repositories/Tgs.GameRecordSchedules.Repositories.csproj", "Tgs.GameRecordSchedules.Repositories/"]
RUN dotnet restore "./Tgs.GameRecordSchedules/./Tgs.GameRecordSchedules.csproj"
COPY . .
WORKDIR "/src/Tgs.GameRecordSchedules"
RUN dotnet build "./Tgs.GameRecordSchedules.csproj" -c $BuildConfiguration -o /app/build

FROM build AS publish
ARG BuildConfiguration
RUN dotnet publish "./Tgs.GameRecordSchedules.csproj" -c $BuildConfiguration -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Tgs.GameRecordSchedules.dll"]