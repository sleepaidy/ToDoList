# 1. Build Stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj files and restore dependencies
COPY ["ToDoList/ToDoList.csproj", "ToDoList/"]
COPY ["ToDoList.Data/ToDoList.Data.csproj", "ToDoList.Data/"]
RUN dotnet restore "ToDoList/ToDoList.csproj"

# Copy everything else and build
COPY . .
WORKDIR "/src/ToDoList"
RUN dotnet publish "ToDoList.csproj" -c Release -o /app/publish /p:UseAppHost=false

# 2. Runtime Stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

# Set the application to listen on port 80 internally
ENV ASPNETCORE_URLS=http://+:80
EXPOSE 80

ENTRYPOINT ["dotnet", "ToDoList.dll"]


