# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["StudentServicePortal/StudentServicePortal.csproj", "StudentServicePortal/"]
RUN dotnet restore "StudentServicePortal/StudentServicePortal.csproj"
COPY . .
WORKDIR "/src/StudentServicePortal"
RUN dotnet build "StudentServicePortal.csproj" -c Release -o /app/build
RUN dotnet publish "StudentServicePortal.csproj" -c Release -o /app/publish

# Stage 2: Run
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/publish .
ENV ASPNETCORE_URLS=http://+:80
ENV ASPNETCORE_ENVIRONMENT=Production
EXPOSE 80
ENTRYPOINT ["dotnet", "StudentServicePortal.dll"]
