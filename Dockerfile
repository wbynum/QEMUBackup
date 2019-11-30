FROM mcr.microsoft.com/dotnet/core/sdk:3.0 AS build
WORKDIR /app

# copy csproj and restore as distinct layers
COPY *.sln .
COPY QEMUBackup/*.csproj ./QEMUBackup/
RUN dotnet restore

# copy everything else and build app
COPY QEMUBackup/. ./QEMUBackup/
WORKDIR /app/QEMUBackup
RUN dotnet publish -c Release -o out


FROM mcr.microsoft.com/dotnet/core/aspnet:3.0 AS runtime
WORKDIR /app
COPY --from=build /app/QEMUBackup/out ./
ENTRYPOINT ["dotnet", "QEMUBackup.dll"]

