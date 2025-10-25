#region License

/*
MIT License

Copyright (c) 2025 Kuro

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
 */

#endregion License

using Ananke.Extensions;
using Ananke.Services.WindowsAPI;
using Ananke.Services.WindowsAPI.Dns;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Scalar.AspNetCore;
using Serilog;

const string APPLICATION_NAME = "Ananke";

// Setup bootstrap logger (https://nblumhardt.com/2020/10/bootstrap-logger/)
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);

    builder.Services.AddSerilog((services, loggerConfiguration) =>
        loggerConfiguration
            .ReadFrom.Configuration(builder.Configuration)
            .ReadFrom.Services(services)
            .Enrich.FromLogContext()); // https://github.com/serilog/serilog/wiki/Enrichment

    builder.Services.AddDataProtection()
        .SetApplicationName(APPLICATION_NAME)
        .ProtectKeysWithDpapiNG();

    // Used for Scalar 
    builder.Services.AddOpenApi();
    
    builder.WebHost.ConfigureKestrel(config =>
        config.ListenAnyIP(port: 21200));

    // Services
    builder.Services.AddSingleton<IProcessTokenManager, ProcessTokenManager>();
    builder.Services.AddHostedService<DiscoveryService>();
    
    var app = builder.Build();

    if (app.Environment.IsDevelopment())
    {
        app.MapOpenApi();
        app.MapScalarApiReference(options =>
        {
            options
                .WithTitle(APPLICATION_NAME)
                .WithTheme(ScalarTheme.DeepSpace)
                .WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.HttpClient);
        });

        Log.Information("Started Scalar service on: http://localhost:21200/scalar");

        app.UseSerilogRequestLogging(options =>
        {
            options.MessageTemplate =
                "HTTP {RequestMethod} {RequestPath} {StatusCode} {Elapsed:0.0000}ms";

            options.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
            {
                diagnosticContext.Set("RequestHost", httpContext.Request.Host.Value!);
                diagnosticContext.Set("RequestScheme", httpContext.Request.Scheme);
            };
        });
    }

    app.UseHttpsRedirection();

    app.UseRouting();

    await app.RunAsync();
}
catch (Exception exception)
{
    Log.Fatal(exception, "Host terminated unexpectedly");
}
finally
{
    await Log.CloseAndFlushAsync();
}