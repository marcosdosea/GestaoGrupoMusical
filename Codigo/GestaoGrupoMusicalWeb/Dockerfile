FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY GestaoGrupoMusicalWeb/GestaoGrupoMusicalWeb.csproj GestaoGrupoMusicalWeb/
RUN dotnet restore GestaoGrupoMusicalWeb/GestaoGrupoMusicalWeb.csproj

COPY . .

RUN dotnet publish GestaoGrupoMusicalWeb/GestaoGrupoMusicalWeb.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "GestaoGrupoMusicalWeb.dll"]
