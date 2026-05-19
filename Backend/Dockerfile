FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 5000

ENV ASPNETCORE_URLS=http://+:5000

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

COPY ["UniParque.API/UniParque.API.csproj", "UniParque.API/"]
COPY ["UniParque.Applicationn/UniParque.Application.csproj", "UniParque.Application/"]
COPY ["UniParque.Domainn/UniParque.Domain.csproj", "UniParque.Domain/"]
COPY ["UniParque.Infrastructuree/UniParque.Infrastructure.csproj", "UniParque.Infrastructure/"]

RUN dotnet restore "UniParque.API/UniParque.API.csproj"

COPY . .

WORKDIR "/src/UniParque.API"
RUN dotnet build "UniParque.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "UniParque.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT [ "dotnet", "UniParque.API.dll" ]