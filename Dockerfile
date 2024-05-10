FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /Navislamia

COPY . ./

WORKDIR /Navislamia/DevConsole
RUN dotnet restore
RUN dotnet publish -c Release -o out

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /Navislamia
COPY --from=build /Navislamia/DevConsole/out .

ENTRYPOINT ["./DevConsole"]