![Logo](./img/peereflits-logo.svg) 

# Peereflits.Shared.Logging


`Peereflits.Shared.Logging` uses [Serilog](https://serilog.net/) as logging engine and can be used in .NET projects from .NET Core 3.1 onwards. The purpose of this package is to apply the (configuration of) logging for all projects in an unambiguous way.

The logging can write logs to three different Serilog "Sinks" (target systems):
1. Console
1. [Logz.io](https://logz.io/)
1. [Application Insights](https://learn.microsoft.com/en-us/azure/azure-monitor/app/app-insights-overview?tabs=net)

When is running in "Development" mode if defaults to `Console` and aborts other configured sinks.

`Peereflits.Shared.Logging` consists of three packages where "Shared" is not used separately:

<!-- Click op de mermaid diagram om deze te openen in https://mermaid.live/ -->
[![](https://mermaid.ink/img/pako:eNo9js0KwjAQBl8l7Ln1AXIQqsWTnip4MD2szTYJND-kyUFK391osXtahoFvFhi8JOCgIgbN7q1wrFzzvHqljFOHB72aEOae1fWRnXbcaYwk-80-7_iS3ZCMd38fKrAULRpZJpavLSBpsiSAl1fSiHlKAoRbi4o5-e7tBuApZqogB4mJWoMlzgIfcZoLJWmSj7ct-1e_fgDkvEFa?type=png)](https://mermaid.live/edit#pako:eNo9js0KwjAQBl8l7Ln1AXIQqsWTnip4MD2szTYJND-kyUFK391osXtahoFvFhi8JOCgIgbN7q1wrFzzvHqljFOHB72aEOae1fWRnXbcaYwk-80-7_iS3ZCMd38fKrAULRpZJpavLSBpsiSAl1fSiHlKAoRbi4o5-e7tBuApZqogB4mJWoMlzgIfcZoLJWmSj7ct-1e_fgDkvEFa)

1. `Logging.WebApps+Logging.Shared` is for (standard) ASP&#46;NET (core) applications;
1. Logging.Functions+Logging.Shared is for use within Azure Functions.

`Logging.Shared` contains the logging setup & configuration used by `Logging.WebApps` and `Logging.Functions`.

## Logging Configuration

1. For console logging set the enviroment variable `Logging:LogTo:Console` to 1.
1. For logging to [Logz.io](https://logz.io/) set the enviroment variable `Logging:LogTo:Logzio` to 1 and provide valid values for environment variables `Logging:Logzio:Token` and `Logging:Logzio:DataCenter:SubDomain`.
1. For logging to [Application Insights](https://learn.microsoft.com/en-us/azure/azure-monitor/app/app-insights-overview?tabs=net) set the enviroment variable `Logging:LogTo:ApplicationInsights` to 1 and configure Application Insights properly in the Azure WebApp (including `APPINSIGHTS_INSTRUMENTATIONKEY` and `APPLICATIONINSIGHTS_CONNECTION_STRING`). It will then log to the AppInsights "Traces".

The default loglevel is `LogEventLevel.Information`. This can be adjusted by providing the environment variable `Logging:LogLevel` with one of the following values:
- Fatal or critical: `LogEventLevel.Fatal`
- Error: `LogEventLevel.Error`
- Warning: `LogEventLevel.Warning`
- Information: `LogEventLevel.Information`
- Debug: `LogEventLevel.Debug`
- Trace of verbose: `LogEventLevel.Verbose`

## Add logging in ASP'NET (core)

The recommended way to apply Serilog is via the `IHostBuilder` extension methods. The extension method `UseSerilog` in `HostBuilderExtensions` from the `Peereflits.Shared.Logging.WebApps` package can be used for this. The example below uses the default settings:

``` csharp
using Peereflits.Shared.Logging;

public class program
{
     public static void Main(string[] args)
     {
         CreateHostBuilder(args).Build().Run();
     }

     private static IHostBuilder CreateHostBuilder(string[] args)
     => return Host.CreateDefaultBuilder(args)
                   .UseSerilog()
                   .ConfigureWebHostDefaults(builder => { builder.UseStartup<Startup>(); });
}
```

The example below uses the default application configuration. After reading the application configuration, the various enrichers and sinks are added to the configuration that is set up by default.

For more information see: [Serilog.Settings.Configuration](https://github.com/serilog/serilog-settings-configuration/blob/dev/README.md)

``` csharp
     Host.CreateDefaultBuilder(args)
         .UseSerilog(useApplicationConfiguration: true)
         .ConfigureWebHostDefaults(builder => { builder.UseStartup<Startup>(); });

```

**Note:** If a sink is added twice, a log will also be written twice.

### Request logging

Serilog can also provide (web-)request logging. To add these, you can use the extension method `UseRequestLogging` in the class `ApplicationBuilderExtensions`. This adds one log line per request containing all relevant information, including time. The duration is relevant for measuring the performance of the APIs and can be found in the field `Elapsed`. In addition, it automatically adds the user agent of the user in the field `RequestUserAgent`.

Adding the request logging middleware should be done as early as possible when configuring this middleware. Only requests handled by handlers added after this middleware are logged. Middleware that handles requests that explicitly should not be logged, such as static files, should stand for this again.

For example:

``` csharp
    if (env.IsDevelopment())
    {
        app.UseDeveloperExceptionPage();
    }
    else
    {
        app.UseExceptionHandler("/Home/Error");
    }
    
    // Middleware die niet gelogd dient te worden zoals static files
    app.UseStaticFiles();
    
    app.UseSerilogRequestLogging();
    // Andere app configuratie en middleware
    ...
```

It is not possible to use request logging if only the different overloads of `IServiceCollection.AddSerilog` are used in this class.

## Add logging in Azure Functions

The recommended way to apply logging is via the `IFunctionsHostBuilder` extension methods. The extension method `UseSerilog` in `FunctionsHostBuilderExtensions` from the `Peereflits.Shared.Logging.Functions` package can be used for this. See the example below:

``` csharp
[assembly: FunctionsStartup(typeof(Startup))]

public class Startup : FunctionsStartup
{
   public override void Configure(IFunctionsHostBuilder builder)
   {
       builder.UseSerilog("My.Application.Name")
               .AddSingleton(...)
               ;
        ...
    }
}
```

In `UseSerilog` the (logical) name of the function must be given as a parameter. The value of this parameter is found in the logging as `ApplicationName`.

### Version support

The libraries supports the following .NET versions:
1. .NET 6.0
1. .NET 7.0
1. .NET 8.0

---

<p align="center">
&copy; No copyright applicable<br />
&#174; "Peereflits" is my codename.
</p>

---
