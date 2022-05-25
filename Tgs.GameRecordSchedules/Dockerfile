#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
ARG BuildConfiguration=Release
RUN --mount=type=secret,id=nuget,dst=/pat cp /pat ~/.nuget/NuGet/NuGet.Config
RUN dotnet nuget remove source 'Microsoft Visual Studio Offline Packages' || true
WORKDIR /src
COPY ["NuGet.config", "."]
COPY ["Tgs.GameRecordSchedules/Tgs.GameRecordSchedules.csproj", "Tgs.GameRecordSchedules/"]
COPY ["Tgs.GameRecordSchedules.Repositories/Tgs.GameRecordSchedules.Repositories.csproj", "Tgs.GameRecordSchedules.Repositories/"]
COPY ["Tgs.GameRecordSchedules.Core/Tgs.GameRecordSchedules.Core.csproj", "Tgs.GameRecordSchedules.Core/"]
RUN dotnet restore "Tgs.GameRecordSchedules/Tgs.GameRecordSchedules.csproj"
COPY . .
WORKDIR "/src/Tgs.GameRecordSchedules"
RUN dotnet build "Tgs.GameRecordSchedules.csproj" -c $BuildConfiguration -o /app/build

FROM build AS publish
ARG BuildConfiguration
RUN dotnet publish "Tgs.GameRecordSchedules.csproj" -c $BuildConfiguration -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Tgs.GameRecordSchedules.dll"]