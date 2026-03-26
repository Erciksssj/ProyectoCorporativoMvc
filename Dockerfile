FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["ProyectoCorporativoMvc.csproj", "./"]
RUN dotnet restore "ProyectoCorporativoMvc.csproj"
COPY . .
RUN dotnet publish "ProyectoCorporativoMvc.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080
COPY --from=build /app/publish .
RUN mkdir -p /app/App_Data /app/wwwroot/uploads/users
ENTRYPOINT ["dotnet", "ProyectoCorporativoMvc.dll"]
