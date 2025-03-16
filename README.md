# Jumpeno

## Description
This monorepo hosts the interactive web game Jumpeno, encompassing both the front-end and back-end components.
The project is developed using the C# framework Blazor.
The application runs in WebAssembly and supports server-side rendering (SSR), which can be enabled through the configuration file.
It also includes support for themes and translations.

The back-end provides a REST API via controllers, which can be accessed using HTTP requests.<br />
In-game communication is handled through SignalR hubs (WebSocket).

The project is divided into client, server and shared part.

## APIDoc (Swagger)
<b>Swagger</b> tool is used for documentation, allowing you to test endpoints, including those requiring authorization. In the development environment, the API documentation is accessible at `/swagger`.

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

There is an utility named ConsoleUI in both Blazor and JavaScript for logging runtime information on specific devices (such as iOS) that cannot be connected to DevTools on our PC. To display this console, add the component to the App.razor and specify its position and dimension parameters like so:

`<ConsoleUI Left="5" Top="5" Width="400" Height="100" />`

Example usage:

`ConsoleUI.WriteLine("Debug info")`

`ConsoleUI.Clear()`

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

## Database
The project uses an <b>SQLite database</b> stored in:
> /Jumpeno.Server/Services/Database/Files/Jumpeno.db

This file is ignored by Git and stores a local copy of the database.<br />
The deployed container uses a separate persistent database from its volume.

To overwrite the deployed database with the next commit, save the file to:
> /Jumpeno.Server/Services/Database/Imports/Jumpeno.db

If <b>Imports</b> folder is empty, volume will not change.<br />
(This process does not work with local build!)

The server database can be accessed and downloaded through the web admin.<br />
To preview the database locally, you can use [DB Browser for SQLite](https://sqlitebrowser.org/).

For database manipulation in code, use <b>Entity Framework</b> and <b>migrations</b>.

## Admin
You can log into the web admin at URL: `/{culture}/admin`.<br />
Here, you can manipulate the database, monitor the application, and run tests.

The application uses a <b>JWT authentication</b> system for both users and administrators.<br />
Administrators are authenticated using email-only authentication with one of the email addresses specified in:
> /Jumpeno.Server/appsettings.json

## Configuration Files
This project includes two configuration files:

For shared common settings:
> /Jumpeno.Server/appsettings.json (Definitions)
> /Jumpeno.Server/Services/Settings/ServerSettings.cs (Use in code)

For server-specific secret configurations:
> /Jumpeno.Shared/appsettings.json (Definitions)
> /Jumpeno.Shared/Services/Settings/AppSettings.cs (Use in code)

## Secrets
Secret information, such as API keys, is defined along with the server configuration in:
> /Jumpeno.Server/appsettings.json

These secrets are initially empty and injected into the Docker image during a job inside GitHub workflows.
For testing purposes, you can create your own local files to override them:
> /Jumpeno.Server/appsettings.Development.json
> /Jumpeno.Server/appsettings.Production.json

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
