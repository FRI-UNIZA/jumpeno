# Jumpeno

## Description
This is the monorepo (FE + BE) for interactive web game Jumpeno written in C# framework Blazor.
Application runs in WebAssembly and has configured SSR with translations.
Backend provides REST API via controllers that should be used with AJAX calls.
Ingame communication is provided by SignalR Hubs.

## Installation
Before you start, please make sure you have installed:
- .NET 8.0
- Microsoft Visual Studio 2022+ or Visual Studio Code

## Run project
To start the project, go to directory:
> /Jumpeno.Server

And run this command:
```dotnet watch```

## Debug
Use built in debugger of your IDE.
To display running application on another device it is recommended to use Dev Tunnels.

## Configuration Files
This project contains two configuration files.
Shared for common config and server for secret (server only) configuration:

> /Jumpeno.Server/appsettings.json

> /Jumpeno.Shared/appsettings.json

## Development workflow ###
After feature implementation, bug fix or release don't forget to raise project Version in:

> /Jumpeno.Shared/appsettings.json

## Learn More
This project was bootstrapped with [Blazor](https://dotnet.microsoft.com/en-us/apps/aspnet/web-apps/blazor).

You can learn more in the [Blazor documentation](https://learn.microsoft.com/sk-sk/aspnet/core/blazor/?view=aspnetcore-8.0&WT.mc_id=dotnet-35129-website).
