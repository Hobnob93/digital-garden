# ===== Build stage =====
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy EVERYTHING first (TODO: optimise later)
COPY . .

#Restore & publish
RUN dotnet restore ./src/DigitalGarden/DigitalGarden.sln
RUN dotnet publish ./src/DigitalGarden/DigitalGarden/DigitalGarden/DigitalGarden.csproj -c Release -o /app/publish /p:UseAppHost=false

# ===== Runtime stage =====
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
ENV ASPNETCORE_URLS=http://+:8080
ENV DOTNET_RUNNING_IN_CONTAINER=true
COPY --from=build /app/publish .
EXPOSE 8080
ENTRYPOINT ["dotnet", "DigitalGarden.dll"]
