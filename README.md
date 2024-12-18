# DotNet Serilog

This project was developed to test serilog in dotnet applications, how to configure and use it and the different ways of viewing the logs.

---

## What is that?

Serilog is a powerful and flexible logging library for .NET applications. It allows you to capture detailed information about your application's execution, such as errors, warnings, information, and metrics, and send it to various destinations, such as files, databases, centralized logging systems (such as Seq), or monitoring services .

## Why use?

- <b>Debug</b>: Quickly identifies the source of errors and problems.
- <b>Monitor</b>: Tracks application performance and health.
- <b>Flexible</b>: Sends logs to various destinations (files, banks, services).
- <b>Customizable</b>: Adds contextual information to logs.

### Implementation

Install

```console
dotnet add package Serilog.AspNetCore --version 8.0.3
```

If you want to implements `Seq`, install:

```console
dotnet add package Serilog.Sinks.Seq --version 8.0.0
```

Create middleware to add trace identifier:

```c#
using Serilog.Context;

namespace <your-name-space>;

public class RequestLogContextMiddleware
{
    private readonly RequestDelegate _next;

    public RequestLogContextMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public Task InvokeAsync(HttpContext context)
    {
        using (LogContext.PushProperty("CorrelationId", context.TraceIdentifier))
        {
            return _next(context);
        }
    }
}
```

In `program.cs` add configs:

```c#
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, loggerConfig) =>
    loggerConfig.ReadFrom.Configuration(context.Configuration));

...

var app = builder.Build();

...

app.UseMiddleware<RequestLogContextMiddleware>();
app.UseSerilogRequestLogging();

...

app.Run();

```

In `appsettings.json` remove default logs configs:

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  }

  ...
}
```

and add the serilog configs:

```json
{
  "Serilog": {
    "Using": [
      "Serilog.Sinks.Console",
      "Serilog.Sinks.Seq"
    ],
    "MinimumLevel": {
      "Default": "Information"
    },
    "WriteTo": [
      {
        "Name": "Console"
      },
      {
        "Name": "File",
        "Args": {
          "path": "../logs/log-.log",
          "rollingInterval": "Day"
        }
      },
      {
        "Name": "Seq",
        "Args": {
          "serverUrl": "http://seq:5341" // To test local using http://localhost:5341
        }
      }
    ]
  },

  ...
}
```

The `WriteTo` configuration defines where you want to write your log, you have several ways to show this, in this example three ways are exemplified, see the example and choose what is best for your case.

### Seq

To see `Seq` logs, go to Url: `http://localhost:5341`

## Test

To run this project you need docker installed on your machine, see the docker documentation [here](https://www.docker.com/).

Having all the resources installed, run the command in a terminal from the root folder of the project and wait some seconds to build project image and download the resources:
`docker-compose up -d`

In terminal show this:

```console
[+] Running 3/3
 ✔ Network dotnet-serilog_default  Created        0.9s
 ✔ Container dotnet_serilog        Started        1.6s
 ✔ Container seq                   Started        2.0s
```

After this, access the link below:

- Swagger project [click here](http://localhost:5000/swagger)

### Stop Application

To stop, run: `docker-compose down`
