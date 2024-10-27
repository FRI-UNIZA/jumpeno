FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ./Jumpeno.Client/Jumpeno.Client.csproj ./Jumpeno.Client/
COPY ./Jumpeno.Server/Jumpeno.Server.csproj ./Jumpeno.Server/
COPY ./Jumpeno.Shared/Jumpeno.Shared.csproj ./Jumpeno.Shared/
COPY ./Jumpeno.sln ./
RUN dotnet restore ./Jumpeno.sln
COPY ./Jumpeno.Shared ./Jumpeno.Shared
COPY ./Jumpeno.Server ./Jumpeno.Server
COPY ./Jumpeno.Client ./Jumpeno.Client
RUN dotnet publish Jumpeno.Server/Jumpeno.Server.csproj -c Release -o /app

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app .
EXPOSE 80
ENTRYPOINT ["dotnet", "JumpenoWebassembly.Server.dll"]
