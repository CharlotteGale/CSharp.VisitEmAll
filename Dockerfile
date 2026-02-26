# Build stage
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /app

COPY ["VisitEmAll/VisitEmAll.csproj", "./VisitEmAll/"]
RUN dotnet restore "VisitEmAll/VisitEmAll.csproj"

COPY . .
WORKDIR "/src/VisitEmAll"
RUN dotnet publish "VisitEmAll.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

ENV ASPNETCORE_HTTP_PORTS=${PORT:-8080}
ENTRYPOINT ["dotnet", "VisitEmAll.dll"]