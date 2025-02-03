# Stage 1: Build the .NET 8 app
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy the project file and restore dependencies.
# Adjust the csproj filename if different.
COPY WebStoreApp.csproj ./
RUN dotnet restore WebStoreApp.csproj

# Copy the rest of the source code and build the app.
COPY . . 
RUN dotnet publish -c Release -o /app/publish

# Stage 2: Create the runtime image.
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "WebStoreApp.dll"]
