
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["SmartScheduler.csproj", "./"]
RUN dotnet restore


COPY . .


RUN dotnet publish "SmartScheduler.csproj" -c Release -o /app/publish --no-restore


FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

ENV ASPNETCORE_URLS=http://+:8080


COPY --from=build /app/publish .


ENTRYPOINT ["dotnet", "SmartScheduler.dll"]
