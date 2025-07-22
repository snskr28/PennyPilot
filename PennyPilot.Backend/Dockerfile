# Use the official .NET SDK image to build and publish the app
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy solution and restore as distinct layers
COPY ["PennyPilot.Backend.Api/PennyPilot.Backend.Api.csproj", "PennyPilot.Backend.Api/"]
COPY ["PennyPilot.Backend.Application/PennyPilot.Backend.Application.csproj", "PennyPilot.Backend.Application/"]
COPY ["PennyPilot.Backend.Domain/PennyPilot.Backend.Domain.csproj", "PennyPilot.Backend.Domain/"]
COPY ["PennyPilot.Backend.Infrastructure/PennyPilot.Backend.Infrastructure.csproj", "PennyPilot.Backend.Infrastructure/"]
COPY . .

WORKDIR "/src/PennyPilot.Backend.Api"
RUN dotnet restore

# Build and publish the app
RUN dotnet publish -c Release -o /app/publish

# Use the official ASP.NET Core runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

# Expose port 80
EXPOSE 80

# Set the entrypoint
ENTRYPOINT ["dotnet", "PennyPilot.Backend.Api.dll"] 