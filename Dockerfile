# ===== Build stage =====
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy sln & csproj into the same relative paths for better layer caching
COPY src/DigitalGarden/DigitalGarden.sln src/DigitalGarden/DigitalGarden.sln
COPY src/DigitalGarden/DigitalGarden.csproj src/DigitalGarden/DigitalGarden.csproj

# Restore (project or solution — either is fine)
RUN dotnet restore ./src/DigitalGarden/DigitalGarden.sln

# Copy the rest of the repo and publish
COPY . .
RUN dotnet publish ./src/DigitalGarden/DigitalGarden/DigitalGarden.csproj -c Release -o /app/publish /p:UseAppHost=false

# ===== Runtime stage =====
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
ENV ASPNETCORE_URLS=http://+:8080
ENV DOTNET_RUNNING_IN_CONTAINER=true
COPY --from=build /app/publish .
EXPOSE 8080
ENTRYPOINT ["dotnet", "DigitalGarden.dll"]
