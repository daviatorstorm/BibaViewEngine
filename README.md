# BibaViewEngine
## Aspnet core view engine

Now Biba view engine is live: https://test-aspnet-core.herokuapp.com/

## Overview

Biba view engine is a new approach of data displaying. Its a brand new engine written from scratch. It is pretty simple have some similarness with angularjs and/or reactjs so if you familiar with those frameworks it will be easy for you to understand how it works.

> Currently it is working with netcoreapp1.1, SDK 1.0.3

## Docs

You can look at working [sample](https://github.com/daviatorstorm/BibaViewEngineSample) with router.

First you need to add Nuget.config file to the root project directory with the next source

```
<?xml version="1.0" encoding="utf-8"?>
<configuration>
  <packageSources>
    <add key="Daviator storm packages" value="https://www.myget.org/F/daviatorstorm/api/v3/index.json" />
  </packageSources>
</configuration>
```

To make a Biba work you must to add Services and pipe to you application in Startup.cs file

Example: 

```
public void ConfigureServices(IServiceCollection services)
{
    ...

    services.AddBibaViewEngine();

    ...
}
```

It will add some main Biba engine services to application DI.

```
public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
{
    ...

    app.UseBibaViewEngine();

    ...
}
```

It will add Biba engine middlewares and features to main application pipeline.

By defalt, when application starts Biba engine take wwwroot/index.html and makes a build wwwroot/index.build.html page that serves to browser.

Now you can add your components.

## Components

To create a component first create a folder calls `Client` in project root directory. There you can create your components.

Add a component called `AppComponent.cs`

Write a next code to that file.

```
using BibaViewEngine;

namespace YourNamespace.Client
{
    public class AppComponent : Component
    {
    }
}
```
Then add a template called `AppComponent.html` in same location and write some html code there

```
<div>
    <h1>Hello World!!!</h1>
</div>
```

Then add you app component like this `<app></app>` to index.html.

```
<!DOCTYPE html>
<html>
<head>
    <meta charset="utf-8" />
    <title>Hello world</title>
</head>
<body>
    <app></app> <!-- Here app component -->
</body>
</html>
```

Now you are ready to start an application.

Enjoy :)
