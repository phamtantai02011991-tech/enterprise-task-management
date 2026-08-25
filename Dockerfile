FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app
COPY . .
RUN dotnet publish src/TaskManagementWeb/TaskManagementWeb.csproj -c Release -o /out

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /out .
EXPOSE 8080 80 10000
ENV ASPNETCORE_URLS=http://+:8080;http://+:80;http://+:10000
ENTRYPOINT ["dotnet", "TaskManagementWeb.dll"]
