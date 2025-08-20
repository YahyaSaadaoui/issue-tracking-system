# build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ../../IssueDesk.sln ./
COPY ../../src/Domain/Domain.csproj src/Domain/
COPY ../../src/Application/Application.csproj src/Application/
COPY ../../src/Infrastructure/Infrastructure.csproj src/Infrastructure/
COPY ../../src/WebApi/WebApi.csproj src/WebApi/
RUN dotnet restore src/WebApi/WebApi.csproj

COPY ../../src ./src
WORKDIR /src/src/WebApi
RUN dotnet publish -c Release -o /app/publish

# run
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
EXPOSE 8080
ENV ASPNETCORE_URLS=http://0.0.0.0:8080
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "WebApi.dll"]
