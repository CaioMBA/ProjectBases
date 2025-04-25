# Use the official .NET 9 ASP.NET runtime image
FROM mcr.microsoft.com/dotnet/aspnet:9.0

#Install Updates/Dependencies
RUN apt-get update \
	&& apt-get install -y jq \
	&& apt-get clean \
	&& rm -rf /var/lib/apt/lists/*

# Set environment variables
ENV ASPNETCORE_ENVIRONMENT=Development
ENV DEPLOYMENT_TARGET=Docker
ENV TZ=UTC
ENV HEALTHCHECK_BASEURL=http://localhost:8080
ENV HEALTHCHECK_PATH=/health

# Set the working directory
WORKDIR /App

EXPOSE 8080

HEALTHCHECK --interval=30s --timeout=5s --start-period=5s --retries=3 \
 CMD curl -fs http://localhost:8080/health | jq -r '.status' | grep -q Healthy

# Set the entry point
ENTRYPOINT ["dotnet", "App.dll"]
