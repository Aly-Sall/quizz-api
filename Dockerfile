# Build Stage
FROM mcr.microsoft.com/dotnet/sdk:6.0-focal AS build
WORKDIR /source

# Copy everything into the container
COPY . .

# Add NSwag for Swagger support
RUN dotnet add ./src/WebUI/WebUI.csproj package NSwag.AspNetCore

# Restore dependencies
RUN dotnet restore "./src/WebUI/WebUI.csproj" --disable-parallel

# Build the project
RUN dotnet build "./src/WebUI/WebUI.csproj" -c Release --no-restore

# Publish the WebUI project
RUN dotnet publish "./src/WebUI/WebUI.csproj" -c Release -o /app --no-restore

# Serve Stage
FROM mcr.microsoft.com/dotnet/aspnet:6.0-focal
WORKDIR /app

# Copy published output from build stage
COPY --from=build /app ./

# Configure environment variables
ENV ASPNETCORE_URLS=http://+:5000
ENV ASPNETCORE_ENVIRONMENT=Development

# Expose port
EXPOSE 5000

# Start the WebUI application
ENTRYPOINT ["dotnet", "_Net6CleanArchitectureQuizzApp.WebUI.dll"]