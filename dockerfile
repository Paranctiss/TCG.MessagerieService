#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 7239
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["NuGet.Config", "./"]
COPY ["NuGet.Config", "TCG.MessagerieService.API/"]
COPY ["NuGet.Config", "TCG.MessagerieService.Application/"]
COPY ["NuGet.Config", "TCG.MessagerieService.Domain/"]
COPY ["NuGet.Config", "TCG.MessagerieService.Persistence/"]
COPY ["TCG.MessagerieService.API/TCG.MessagerieService.API.csproj", "TCG.MessagerieService.API/"]
COPY ["TCG.MessagerieService.Application/TCG.MessagerieService.Application.csproj", "TCG.MessagerieService.Application/"]
COPY ["TCG.MessagerieService.Domain/TCG.MessagerieService.Domain.csproj", "TCG.MessagerieService.Domain/"]
COPY ["TCG.MessagerieService.Persistence/TCG.MessagerieService.Persistence.csproj", "TCG.MessagerieService.Persistence/"]
RUN dotnet restore "TCG.MessagerieService.API/TCG.MessagerieService.API.csproj"
RUN dotnet restore "TCG.MessagerieService.Application/TCG.MessagerieService.Application.csproj"
RUN dotnet restore "TCG.MessagerieService.Domain/TCG.MessagerieService.Domain.csproj"
RUN dotnet restore "TCG.MessagerieService.Persistence/TCG.MessagerieService.Persistence.csproj"
COPY . .
WORKDIR "/src/TCG.MessagerieService.API"
RUN dotnet build "TCG.MessagerieService.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "TCG.MessagerieService.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TCG.MessagerieService.API.dll"]