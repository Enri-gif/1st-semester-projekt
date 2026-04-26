FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY Shared/Shared.csproj Shared/
COPY blazor/blazor/blazor.csproj blazor/blazor/
RUN dotnet restore blazor/blazor/blazor.csproj

COPY Shared/ Shared/
COPY blazor/blazor/ blazor/blazor/
RUN dotnet publish blazor/blazor/blazor.csproj -c Release -o /app/publish

FROM nginx:alpine AS runtime
COPY --from=build /app/publish/wwwroot /usr/share/nginx/html
EXPOSE 80
