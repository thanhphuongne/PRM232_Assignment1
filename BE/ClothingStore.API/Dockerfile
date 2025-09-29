# Use the official .NET 8 SDK image to build the app
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy the project file and restore dependencies
COPY BE/ClothingStore.API/ClothingStore.API.csproj .
RUN dotnet restore

# Copy the rest of the source code
COPY . .

# Build and publish the app
RUN dotnet publish -c Release -o /app/publish

# Use the official .NET 8 ASP.NET runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Copy the published app from the build stage
COPY --from=build /app/publish .

# Expose the port the app runs on
EXPOSE 80

# Set the entry point
ENTRYPOINT ["dotnet", "ClothingStore.API.dll"]