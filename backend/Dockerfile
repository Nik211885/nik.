# Pre-built locally with: dotnet publish backend.csproj -c Release -o ./publish
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app

COPY publish .

EXPOSE 5055
ENV ASPNETCORE_URLS=http://+:5055

ENTRYPOINT ["dotnet", "backend.dll"]
