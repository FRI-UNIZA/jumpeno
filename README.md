# Jumpeno

## Description
This monorepo hosts the interactive web game Jumpeno, encompassing both the front-end and back-end components.
The project is developed using the C# framework Blazor.
The application runs in WebAssembly and supports server-side rendering (SSR), which can be enabled through the configuration file.
It also includes support for themes and translations.

The back end provides a REST API via controllers, which can be accessed using HTTP requests.

In-game communication is handled through SignalR hubs.

## Installation
Before you start, please make sure you have installed:
- .NET 8.0
- Microsoft Visual Studio 2022+ or Visual Studio Code

## Run
To start the project, go to directory:
> /Jumpeno.Server

and run this command:
`dotnet watch`

## Debug
Use the built-in debugger in your IDE.

Ensure that `Jumpeno.sln` is selected as your workspace solution.

To access the running application on a different device, it is recommended to use `DevTunnels`.

## Build
To build the project, go to root directory:
> /

Create release build:
`dotnet publish -c Release -o Publish`

Navigate to publish directory:
> /Publish

Run the project using dotnet:
`dotnet Jupeno.Server.dll`

## Run in Docker
Docker desktop app must be installed!

To build the project, go to root directory:
> /

Delete `bin` and `obj` directories:
> /Jumpeno.Client/bin & /Jumpeno.Client/obj

> /Jumpeno.Server/bin & /Jumpeno.Server/obj

> /Jumpeno.Shared/bin & /Jumpeno.Shared/obj

Create image (release build):
`docker build -t jumpeno . --no-cache`

Start the container:
`docker-compose up`

## CI/CD
There is a CI/CD pipeline configured using GitHub Actions:

For the `master` branch, deployed at: "https://jumpeno.fri.uniza.sk/".
> /github/workflows/docker_latest.yml

For the `development` branch, deployed at: "https://devjumpeno.docker.kst.fri.uniza.sk/".
> /github/workflows/docker_dev.yml

When deployed, the application runs as a Docker container created from a release build image.

Docker configuration can be modified in:

> /Dockerfile

> /docker-compose.yml

App must run on port `80` to work properly (configured in `appsettings.json`).

## Configuration Files
This project includes two configuration files:

For shared common settings:
> /Jumpeno.Server/appsettings.json

For server-specific secret configurations:
> /Jumpeno.Shared/appsettings.json

## Scripts
Build scripts are located in the following directory:

> /Scripts

These programs can run automatically as part of the project’s build and run actions.

For example, ThemeProvider automatically generates theme CSS variables from C# code.

## Development Workflow ###
After implementing features, fixing bugs, or releasing updates, remember to update project version in the file:

> /Jumpeno.Shared/appsettings.json

## Learn More
This project was bootstrapped with [Blazor](https://dotnet.microsoft.com/en-us/apps/aspnet/web-apps/blazor).

You can learn more in the [Blazor documentation](https://learn.microsoft.com/sk-sk/aspnet/core/blazor/?view=aspnetcore-8.0&WT.mc_id=dotnet-35129-website).
