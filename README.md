# Jumpeno

## Description
This <b>monorepo</b> hosts the interactive web game <b>Jumpeno</b>.
(both front-end and back-end part.)<br />
The project is developed using <b>C# framework Blazor</b>.<br />
Application runs in <b>WebAssembly</b> and supports server-side rendering <b>(SSR)</b>, which can be enabled in configuration file.
It also includes support for <b>themes</b> and <b>translations</b>.

The back-end provides a <b>REST API</b> via controllers, which can be accessed using HTTP requests.<br />
In-game communication is handled through <b>SignalR</b> hubs (WebSocket).

The solution is divided into <b>client</b> and <b>server</b> part.<br />
Shared functionality is contained in the <b>client</b> project.

Services like <b>MariaDB</b>, <b>Adminer</b> and <b>MailCatcher</b> run inside <b>Docker</b>.

## Installation
Please make sure you have installed:
- [.NET 9.0.305 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/9.0)
- [Docker Desktop](https://www.docker.com/products/docker-desktop/)
- [Microsoft Visual Studio 2022+ or Visual Studio Code](https://visualstudio.microsoft.com/downloads/)
- Project workloads (Run in terminal: `dotnet workload restore`)

## Before you start (Docker)
To provide server-like environment, all parts of the application run in <b>Docker</b>.<br />
Another advantage is that it is the only mandatory tool except .NET and IDE for local development.<br />
Whether app runs in hot reloading or is built for debug/production, it connects to running containers for <b>MariaDB</b> and <b>MailCatcher</b>.<br />
Before you start the project, ensure that containers are running.<br />
<b>Always do these steps first</b>:

[1] Start <b>Docker Desktop</b> app.

[2] Go to root directory:
> /

[3] Build images (skip if you have the latest build):
`docker-compose build --no-cache`

[4] Run containers:
`docker-compose up` (or in app GUI)

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

The app is now accessible from other devices via the URL: "https://192.168.1.12:7284".

(Replace `192.168.1.12` with the local IP address of your computer)

## Local network (Docker)
You can run desktop Docker container on "http://localhost:80".

Certain features require HTTPS!<br />
In case you want to access it from other device, setup reverse proxy with self signed SSL/TLS certificate.

## Debug
Use the built-in debugger in your IDE.<br />
For `Release` configuration temporarily disable `bundle` option in:
> /Jumpeno.Client/AppSettings.Client.json

Ensure that `Jumpeno.sln` is selected as your workspace solution.

To access the running application on a different device,<br />
it is recommended to use `DevTunnels` or `Local network` option.

### ConsoleUI
There is an utility named <b>ConsoleUI</b> in both <b>Blazor</b> and <b>JavaScript</b> for logging runtime information on specific devices (such as iOS) that cannot be connected to DevTools on our PC. To display this console, add the component to the App.razor and specify its position and dimension parameters like so:

`<ConsoleUI Left="5" Top="5" Width="400" Height="100" />`

Example usage:

`ConsoleUI.WriteLine("Debug info")`

`ConsoleUI.Clear()`

## Build
Temporarily disable `Bundle` option in:
> /Jumpeno.Client/AppSettings.Client.json

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

Delete created images and containers in Docker Desktop. (optional)

To build the project, go to root directory:
> /

Create images (release build):
`docker-compose build --no-cache`

Start the containers:
`docker-compose up`

## Database
### MariaDB
This project uses <b>MariaDB</b> running on port `3306`.<br />
DB logic and all local files are stored under:
> /Jumpeno.Server/Services/Database

### Adminer
For data preview and manipulation use <b>Adminer</b>.<br />
This service runs locally on `{web-url}:8080`.<br />
You can also log in through Jumpeno web admin.

### Entity Framework
App uses <b>Entity Framework</b> ORM.<br />
Please install: `dotnet tool install --global dotnet-ef`.<br />
To change data model via code, follow these steps:

[1] Update <b>entities</b>:
> /Jumpeno.Server/Services/Database/Entities <br />
> /Jumpeno.Server/Services/Database/DB.cs

[2] Go to server directory <b>(App must not be running!)</b>:
> /Jumpeno.Server

[3] Create new <b>migration</b>: `dotnet ef migrations add {MigrationName}`<br />
All migrations are stored in:
> /Jumpeno.Server/Services/Database/Migrations

[4] Migrations run automatically on app start.<br />
(Ensure that new migration can be applied to existing data on the server!)<br />
(To achieve it, you can also tweak the migration in `{MigrationName}.cs` file.)

## Admin
You can log into the web as administrator.<br />
Here, you can manipulate the database, monitor the application, and run tests.

The application uses a <b>OAuth</b> with <b>JWT</b> for both users and administrators.<br />
Administrators are authenticated email-only with addresses specified in:
> /Jumpeno.Server/AppSettings.Server.json

## Email
<b>Admin login</b>, <b>activation</b> and <b>password reset</b> links are sent to email.<br />
Production and development server uses <b>GMail SMTP</b>.<br />
There is a container named <b>MailCatcher</b> to view emails in local development.<br />
To send emails to <b>MailCatcher</b> use port `1025`.<br />
To view emails go to `{web-url}:1080`.

## Design system
All components are designed for reusability and styled using <b>theme variables</b> for any surface. <b>Component that creates surface has surface class</b> which provides all the <b>CSS variables</b> for given surface.

You can define surfaces in the `SURFACE` enum, then use `__SURFACE` suffix in theme definition to set colors and shadows specifically for that surface.

To style a component, place it temporarily in:
> DesignerPage > Playground

To see the outcome on all possible surfaces, visit:
`/en/designer` or `/sk/designer`.

<b>Recommended:</b> To set component classes, utilize methods of <b>CSSClass!</b>

## APIDoc (Swagger)
<b>Swagger</b> tool is used for documentation.

This allows you to test API endpoints, including those requiring authorization.

In the development environment, the API documentation is accessible at URL: `/swagger`.

## CI/CD
There is a CI/CD pipeline configured using GitHub Actions:

For the `master` branch, deployed at: "https://jumpeno.fri.uniza.sk/".
> /github/workflows/docker_latest.yml

For the `development` branch, deployed at: "https://devjumpeno.docker.kst.fri.uniza.sk/".
> /github/workflows/docker_dev.yml

When deployed, the application runs as a Docker containers created from a release build images.

Containers can be configured in:

> /Dockerfile.database<br />
> /Dockerfile.jumpeno<br />
> /Dockerfile.mailcatcher<br />
> /Dockerfile.adminer

> /docker-compose.yml

App is served on port mentioned in `Settings` section!

## Configuration files
This project includes 3 configuration files:

For shared common settings:
> /Jumpeno.Client/AppSettings.Client.json (Definitions)<br />
> /Jumpeno.Client/Services/Settings/AppSettings.cs (Use in code)

For server-specific secret configurations:
> /Jumpeno.Server/AppSettings.Server.json (Definitions)<br />
> /Jumpeno.Server/Services/Settings/ServerSettings.cs (Use in code)

<b>Environment variables</b> for services like database and email are set in:
> .env

## Secrets
<b>Secret information</b>, such as API keys, is defined together with server config in:
> /Jumpeno.Server/AppSettings.Server.json

These secrets are automatically initialized for development.<br />
Server configuration is injected into the Docker image during a job inside GitHub workflows.<br />

## Settings
Deployed app must run on port `80` to work properly!<br />
(`Port` option in `/Jumpeno.Server/AppSettings.Server.json`).

In latest .NET version "SIMD" is enabled automatically,<br />
howewer must be turned off to support older mobile phone devices.<br />
(`WasmEnableSIMD` option in `/Jumpeno.Client/Jumpeno.Client.csproj`)

Not that client part has "tree shaking" enabled to speed up initial loading time.<br />
(`PublishTrimmed` option in `/Jumpeno.Client/Jumpeno.Client.csproj`)

Static CSS and JS files in `Jumpeno.Client/wwwroot` directory should also be bundled for production.<br />
(`Bundle` option in `/Jumpeno.Client/AppSettings.Client.json`)

SSR can be enabled, but not recommended.<br />
(`Prerender` option in `/Jumpeno.Client/AppSettings.Client.json`)

Settings include automatic redirect to and out of authorized pages.<br />
(`Redirect` option in `/Jumpeno.Client/AppSettings.Client.json`)

After feature implementation or bugfix, do not forget to update project version.<br />
(`Version` option in `/Jumpeno.Client/AppSettings.Client.json`)

Additional options like language settings can be set in `/Jumpeno.Client/AppSettings.Client.json`.

## Scripts
Build scripts are located in the following directory:

> /Scripts

These programs can run automatically as part of the project’s build and run actions.

For example, `ThemeProvider` automatically generates theme CSS variables from C# code.

## Learn more
This project is developed with [Blazor](https://dotnet.microsoft.com/en-us/apps/aspnet/web-apps/blazor).

You can learn more in the [Blazor documentation](https://learn.microsoft.com/en-us/aspnet/core/blazor/?view=aspnetcore-9.0).
