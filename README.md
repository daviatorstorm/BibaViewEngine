# BibaViewEngine
## Aspnet core view engine

## Overview

Biba view engine is a new approach of data displaying. Its a brand new engine written from scratch. It is pretty simple have some similarness with angularjs or reactjs so if you familiar with those frameworks it will be easy for you to understand how it works.

> Currently it is working with netcoreapp1.1, SDK 1.0.3 / 1.0.4
> Updated to netcoreapp2.0 and SDK 2.0.0

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

> Note
> 
> Every component must inherit `BibaViewEngine.Component` base class, because, BibaEngine searches component classes by this base class and it have a base implementation behavior that needed for every component.

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

After this you can start an application.

## Data Binding

### Simple binding

Lets take a look at the next step - `Data Binding`. For now, you can bind only simple data, such as nummerics, strings and booleans. Lets see how it works.
First modify `Client/AppComponent.cs` and `Client/AppComponent.html`

```
Client/AppComponent.cs
------------------------------------------------------------------
using BibaViewEngine;

namespace BibaViewEngineTutorial.Client
{
    public class AppComponent : Component
    {
        public string HelloWorld { get; set; } = "Hello World!";
    }
}

Client/AppComponent.html
------------------------------------------------------------------
<h1>([helloworld])</h1>
```

And start an application.

Simple :)

### Component data binding

There is a posibility to send data from parent to child component.
For this we need to create another component. Lets call it `SimpleBindComponent`.

Example

```
Client/SimpleBindComponent.cs
------------------------------------------------------------------
using BibaViewEngine;
using BibaViewEngine.Attributes;

namespace BibaViewEngineTutorial.Client
{
    public class SimpleBindComponent : Component
    {
        [Input]
        public string Message { get; set; }
    }
}

Client/SimpleBindComponent.html
------------------------------------------------------------------
<p>([message])</p>
```

For data bindin component-component you need to set an `Input` attribute for property that you want ot bind.
In our example its `Message`.

Then make some modifications in `AppComponent.html`.

```
Client/AppComponent.cs
------------------------------------------------------------------
using BibaViewEngine;

namespace BibaViewEngineTutorial.Client
{
    public class AppComponent : Component
    {
        public string HelloWorld { get; set; } = "Hello World!";

        public string Message { get; set; } = "My name is John Doe"; // Added this property
    }
}

Client/AppComponent.html
------------------------------------------------------------------
<h1>([helloworld])</h1>
<simplebind message="message"></simplebind> <!-- Added this component -->
```

Start an application.

## Routing

Every time that app executes - BibaEngine injecting a routing script to html.
That why engine generates `wwwroot/index.build.html`.
You can build an application and watch to `wwwroot/index.build.html` and find a script appended to the top of head element.

So now we will use it.

An `AddBibaViewEngine()` have some parameters:
`BibaViewEngine.Router.Routes` and `BibaViewEngine.Models.BibaViewEnginePropperties` that are equals to null.
Then method to set these to default values.

In our application we will create one route and look how it works.
For doing that we must to create, for example, `Client/MainComponent.cs`, `Client/MainComponent.html` and modify a `Startup.cs` and `wwwroot/index.html`.

```
Client/MainComponent.cs
------------------------------------------------------------------
using BibaViewEngine;

namespace BibaViewEngineTutorial.Client
{
    public class MainComponent : Component
    {
        public string Message { get; set; } = "My name is John Doe";
    }
}


Client/MainComponent.html
------------------------------------------------------------------
<simplebind message="message"></simplebind>
```

Then remove message property from `AppComponent.cs` and remove `<simplebind message="message"></simplebind>` line from `AppComponent.html` and the the next line `<div router-container></div>`.
That line means that this is a container for all routes that you will define.

So files will looks like this.

```
Client/AppComponent.cs
------------------------------------------------------------------
using BibaViewEngine;

namespace BibaViewEngineTutorial.Client
{
    public class AppComponent : Component
    {
        public string HelloWorld { get; set; } = "Hello World!";
    }
}

Client/AppComponent.html
------------------------------------------------------------------
<h1>([helloworld])</h1>
<div router-container></div>
```

At last we just have to add a route with `MainComponent` class in `Startup.cs`.

```
public void ConfigureServices(IServiceCollection services)
{
    var routes = new Routes
    {
        new BibaRoute { Path= "", Component = typeof(MainComponent) },
    };

    services.AddBibaViewEngine(routes: routes);
}
```

Start application.

Great! Everything works as we expected.

One route is good, but not enough to show what router can.
Now we will try to move between routes.
For this we must to add another route.

Create another one component called `ListComponent`.

```
Client/ListComponent.cs
------------------------------------------------------------------
using BibaViewEngine;
using System.Collections.Generic;

namespace BibaViewEngineTutorial.Client
{
    public class ListComponent : Component
    {
        public IEnumerable<string> Values { get; set; } = new string[] { "Value1", "Value2", "Value3" };
    }
}

Client/ListComponent.html
------------------------------------------------------------------
<h2>Some list with for loop</h2>
<ul>
    <for iter="item" source="Values" element="li">
        ([item])
    </for>
</ul>
```

Then add this to routes
```
public void ConfigureServices(IServiceCollection services)
{
    var routes = new Routes
    {
        new BibaRoute { Path= "", Component = typeof(MainComponent) },
        new BibaRoute { Path= "list", Component = typeof(ListComponent) } // Added this
    };

    services.AddBibaViewEngine(routes: routes);
}
```

And create links for those roues in `AppComponent.html`.
```
<h1>([helloworld])</h1>

<div>
    <a router-path="">Main</a>
    <a router-path="list">List</a>
</div>

<div router-container></div>
```

Found a problem - [post issue](https://github.com/daviatorstorm/BibaViewEngine/issues).

Want to contribute - write me an email here [rayan.de.bum@gmail.com](mailto:rayan.de.bum@gmail.com).

Enjoy :)
