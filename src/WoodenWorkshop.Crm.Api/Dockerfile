FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["src/WoodenWorkshop.Crm.Api/WoodenWorkshop.Crm.Api.csproj", "WoodenWorkshop.Crm.Api/"]
RUN dotnet restore "src/WoodenWorkshop.Crm.Api/WoodenWorkshop.Crm.Api.csproj"
COPY . .
WORKDIR "/src/WoodenWorkshop.Crm.Api"
RUN dotnet build "WoodenWorkshop.Crm.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "WoodenWorkshop.Crm.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "WoodenWorkshop.Crm.Api.dll"]
