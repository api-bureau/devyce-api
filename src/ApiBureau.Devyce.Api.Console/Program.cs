using ApiBureau.Devyce.Api.Console;
using ApiBureau.Devyce.Api.Console.Services;
using ApiBureau.Devyce.Api.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddSerilog((services, configuration) =>
{
    configuration
        .ReadFrom.Configuration(builder.Configuration)
        .ReadFrom.Services(services)
        .Enrich.FromLogContext();
});

builder.Services.AddDevyce(builder.Configuration);
builder.Services.AddScoped<DataService>();

using var host = builder.Build();

var cliConfiguration = new CommandLineConfiguration(host.Services);
var rootCommand = cliConfiguration.CreateRootCommand();

var parseResult = rootCommand.Parse(args);
return parseResult.Invoke();