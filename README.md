# Jumpeno

## Description
This monorepo hosts the interactive web game Jumpeno, encompassing both the front-end and back-end components.
The project is developed using the C# framework Blazor.
The application runs in WebAssembly and supports server-side rendering (SSR), which can be enabled through the configuration file.
It also includes support for themes and translations.

The back-end provides a REST API via controllers, which can be accessed using HTTP requests.<br />
In-game communication is handled through SignalR hubs.

The project is divided into client, server and shared part.

## Installation
Before you start, please make sure you have installed:
- .NET 8.0
- Microsoft Visual Studio 2022+ or Visual Studio Code
- You can be prompted to install additional workloads

## Run (Hot reloading)
To simply start the project, go to directory:
> /Jumpeno.Server

And run this command:
`dotnet watch`

## Local network
To test the app on a local network, open port `7284` through the firewall.

Go to directory:
> /Jumpeno.Server

And run this command:
`dotnet watch --urls "https://192.168.1.12:7284"`

The app is now accessible on other devices via the URL: "https://192.168.1.12:7284".

(Replace `192.168.1.12` with the local IP address of your computer)

## Debug
Use the built-in debugger in your IDE.

Ensure that `Jumpeno.sln` is selected as your workspace solution.

To access the running application on a different device, it is recommended to use `DevTunnels` or `Local network` option.

## Build
Temporarily disable `Bundle` option in:
> /Jumpeno.Shared/appsettings.json

Go to root directory:
> /

Create release build:
`dotnet publish -c Release -o Publish`

Navigate to publish directory:
> /Publish

Run the project using dotnet:
`dotnet Jumpeno.Server.dll`

## Run in Docker
Docker desktop app must be installed and running!

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

App is served on port mentioned in `Settings` section!

## Configuration Files
This project includes two configuration files:

For shared common settings:
> /Jumpeno.Server/appsettings.json

For server-specific secret configurations:
> /Jumpeno.Shared/appsettings.json

## Settings
Deployed app must run on port `80` to work properly!<br />
(`Port` option in `/Jumpeno.Server/appsettings.json`).

In latest .NET version "SIMD" is enabled automatically,<br />
howewer must be turned off to support older mobile phone devices.<br />
(`WasmEnableSIMD` option in `/Jumpeno.Client/Jumpeno.Client.csproj`)

Not that client part has "tree shaking" enabled to speed up initial loading time.<br />
(`PublishTrimmed` option in `/Jumpeno.Client/Jumpeno.Client.csproj`)

Static CSS and JS files in `Jumpeno.Client/wwwroot` directory should also be bundled for production.<br />
(`Bundle` option in `/Jumpeno.Shared/appsettings.json`)

SSR can be enabled, but not recommended.<br />
(`Prerender` option in `/Jumpeno.Shared/appsettings.json`)

After feature implementation or bugfix, do not forget to update project version.<br />
(`Version` option in `/Jumpeno.Shared/appsettings.json`)

Additional options like language settings can be set in `/Jumpeno.Shared/appsettings.json`.

## Scripts
Build scripts are located in the following directory:

> /Scripts

These programs can run automatically as part of the project’s build and run actions.

For example, `ThemeProvider` automatically generates theme CSS variables from C# code.

## Learn More
This project was bootstrapped with [Blazor](https://dotnet.microsoft.com/en-us/apps/aspnet/web-apps/blazor).

You can learn more in the [Blazor documentation](https://learn.microsoft.com/en-us/aspnet/core/blazor/?view=aspnetcore-8.0&WT.mc_id=dotnet-35129-website).
