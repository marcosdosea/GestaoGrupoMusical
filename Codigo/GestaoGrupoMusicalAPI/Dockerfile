FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY GestaoGrupoMusicalAPI/GestaoGrupoMusicalAPI.csproj GestaoGrupoMusicalAPI/
RUN dotnet restore GestaoGrupoMusicalAPI/GestaoGrupoMusicalAPI.csproj

COPY . .

RUN dotnet publish GestaoGrupoMusicalAPI/GestaoGrupoMusicalAPI.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "GestaoGrupoMusicalAPI.dll"]
