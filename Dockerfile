# -------- build --------
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copiamos los .csproj primero para cachear el restore
COPY ProyectoForex.sln ./
COPY src/Api/ProyectoForex.Api.csproj src/Api/
COPY src/Application/ProyectoForex.Application.csproj src/Application/
COPY src/Domain/ProyectoForex.Domain.csproj src/Domain/
COPY src/Infrastructure/ProyectoForex.Infrastructure.csproj src/Infrastructure/

RUN dotnet restore src/Api/ProyectoForex.Api.csproj

# Ahora copiamos el resto del código
COPY . .
RUN dotnet publish src/Api/ProyectoForex.Api.csproj -c Release -o /app/out

# -------- runtime --------
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/out .

# Render inyecta ${PORT}; exponemos 8080 internamente
ENV ASPNETCORE_URLS=http://0.0.0.0:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "ProyectoForex.Api.dll"]
