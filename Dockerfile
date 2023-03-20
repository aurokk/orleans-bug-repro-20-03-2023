FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build

WORKDIR /source

COPY *.sln                     .
COPY src/ClassLibrary/*.csproj ./src/ClassLibrary/
COPY src/ConsoleApp1/*.csproj  ./src/ConsoleApp1/

RUN dotnet restore

COPY src/ ./src

RUN dotnet publish \
    -c Release \
    -o /app \
    --no-restore \
    src/ConsoleApp1/ConsoleApp1.csproj

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS runtime

WORKDIR /app

COPY --from=build /app ./

ENV ASPNETCORE_URLS=http://+:80

ENTRYPOINT ["dotnet", "ConsoleApp1.dll"]