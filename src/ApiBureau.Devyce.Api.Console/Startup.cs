using ApiBureau.Devyce.Api.Console.Services;
using ApiBureau.Devyce.Api.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace ApiBureau.Devyce.Api.Console;

public class Startup
{
    private IConfigurationRoot Configuration { get; }

    public Startup()
    {
        var builder = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", true)
            .AddUserSecrets<Startup>()
            .AddEnvironmentVariables();

        Configuration = builder.Build();
    }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddLogging(configure =>
        {
            configure.AddConfiguration(Configuration.GetSection("Logging"));
            configure.AddConsole();
        });
        services.AddDevyce(Configuration);
        services.AddScoped<DataService>();
    }
}