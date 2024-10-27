# Jumpeno

## Description
This is the monorepo for the interactive web game Jumpeno, which includes both the front end and back end.
The project is built using the C# framework, Blazor.
The application runs in WebAssembly and has server-side rendering (SSR) that can be configured to be turned on or off in the configuration file.
It also supports themes and translations.

The backend offers a REST API through controllers, which should be utilized with AJAX calls.
In-game communication is facilitated by SignalR hubs.

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
Use built-in debugger of your IDE.
To display a running application on a different device, it is recommended to use DevTunnels.

## Configuration Files
This project contains two configuration files:
one for shared common settings and another for server-specific secret configurations:

> /Jumpeno.Server/appsettings.json

> /Jumpeno.Shared/appsettings.json

## Development workflow ###
After implementing features, fixing bugs, or releasing updates, remember to raise the project version in the file:

> /Jumpeno.Shared/appsettings.json

## Learn More
This project was bootstrapped with [Blazor](https://dotnet.microsoft.com/en-us/apps/aspnet/web-apps/blazor).

You can learn more in the [Blazor documentation](https://learn.microsoft.com/sk-sk/aspnet/core/blazor/?view=aspnetcore-8.0&WT.mc_id=dotnet-35129-website).
