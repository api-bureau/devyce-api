using ApiBureau.Devyce.Api.Console;
using ApiBureau.Devyce.Api.Console.Services;
using ApiBureau.Devyce.Api.Extensions;
using Azure.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;

var builder = Host.CreateApplicationBuilder(args);

// Optional: set KeyVault:VaultUri (user secrets or KeyVault__VaultUri) to load the settings from Azure Key Vault
var vaultUri = builder.Configuration["KeyVault:VaultUri"];

if (!string.IsNullOrWhiteSpace(vaultUri))
{
    if (!Uri.TryCreate(vaultUri, UriKind.Absolute, out var uri) || uri.Scheme != Uri.UriSchemeHttps)
    {
        throw new InvalidOperationException("KeyVault:VaultUri must be an absolute HTTPS URI.");
    }

    // Loaded last, so vault secrets override appsettings and user secrets
    builder.Configuration.AddAzureKeyVault(uri, new DefaultAzureCredential());
}

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
return await parseResult.InvokeAsync();