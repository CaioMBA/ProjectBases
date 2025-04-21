# Use the official .NET 9 ASP.NET runtime image
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base

# Set environment to Development
ENV ASPNETCORE_ENVIRONMENT=Development

# Set timezone
ENV TZ="America/Sao_Paulo"


# Set the working directory
WORKDIR /App

# Expose necessary ports
EXPOSE 8080
EXPOSE 8081

# Set the entry point
ENTRYPOINT ["dotnet", "App.dll"]
