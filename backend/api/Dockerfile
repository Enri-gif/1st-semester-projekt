FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY Shared/Shared.csproj Shared/
COPY backend/api/api.csproj backend/api/
RUN dotnet restore backend/api/api.csproj

COPY Shared/ Shared/
COPY backend/api/ backend/api/
RUN dotnet publish backend/api/api.csproj -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish ./

ENV ASPNETCORE_URLS=http://+:5000
EXPOSE 5000

ENTRYPOINT ["dotnet", "api.dll"]
