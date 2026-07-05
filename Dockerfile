# ---- Build stage ----
# Full SDK image: restores dependencies and compiles/publishes the app.
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy the solution and every .csproj first, then restore. This layer is cached
# and only re-runs when a project file changes (not on every source edit).
COPY CatalogoMultiTenant.sln ./
COPY src/CatalogoMultiTenant.Domain/CatalogoMultiTenant.Domain.csproj                 src/CatalogoMultiTenant.Domain/
COPY src/CatalogoMultiTenant.Application/CatalogoMultiTenant.Application.csproj       src/CatalogoMultiTenant.Application/
COPY src/CatalogoMultiTenant.Infrastructure/CatalogoMultiTenant.Infrastructure.csproj src/CatalogoMultiTenant.Infrastructure/
COPY src/CatalogoMultiTenant.WebApi/CatalogoMultiTenant.WebApi.csproj                 src/CatalogoMultiTenant.WebApi/
RUN dotnet restore CatalogoMultiTenant.sln

# Copy the rest of the source and publish the WebApi project in Release mode.
COPY . .
RUN dotnet publish src/CatalogoMultiTenant.WebApi/CatalogoMultiTenant.WebApi.csproj \
    -c Release -o /app/publish --no-restore

# ---- Runtime stage ----
# Slim ASP.NET runtime image: no SDK, only what is needed to run the app.
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish ./

ENV ASPNETCORE_ENVIRONMENT=Production
# Documented default; Render overrides the actual port via the PORT env var below.
EXPOSE 8080

# Render assigns the port dynamically through $PORT. Bind Kestrel to 0.0.0.0:$PORT
# at runtime (fallback 8080 for local `docker run`). `exec` hands the process over
# to dotnet so it receives SIGTERM for graceful shutdown.
ENTRYPOINT ["sh", "-c", "ASPNETCORE_URLS=http://0.0.0.0:${PORT:-8080} exec dotnet CatalogoMultiTenant.WebApi.dll"]
