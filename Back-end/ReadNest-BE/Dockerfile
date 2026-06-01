FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["ReadNest-BE/ReadNest-BE.csproj", "ReadNest-BE/"]
COPY ["ReadNest-Model/ReadNest-Model.csproj", "ReadNest-Model/"]

RUN dotnet restore "ReadNest-BE/ReadNest-BE.csproj"

COPY . .

WORKDIR "/src/ReadNest-BE"
RUN dotnet build "ReadNest-BE.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ReadNest-BE.csproj" -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app

COPY --from=publish /app/publish .

RUN mkdir -p Uploads/images data

EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

ENTRYPOINT ["dotnet", "ReadNest-BE.dll"]
