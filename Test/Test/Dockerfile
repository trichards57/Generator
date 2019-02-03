FROM microsoft/dotnet:2.1-aspnetcore-runtime-nanoserver-1709 AS base
WORKDIR /app
EXPOSE 57907
EXPOSE 44378

FROM microsoft/dotnet:2.1-sdk-nanoserver-1709 AS build
WORKDIR /src
COPY Test/Test.csproj Test/
RUN dotnet restore Test/Test.csproj
COPY . .
WORKDIR /src/Test
RUN dotnet build Test.csproj -c Release -o /app

FROM build AS publish
RUN dotnet publish Test.csproj -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Test.dll"]
