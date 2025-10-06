using ApiBureau.Devyce.Api.Console;
using ApiBureau.Devyce.Api.Console.Services;
using Microsoft.Extensions.DependencyInjection;

var services = new ServiceCollection();

var startup = new Startup();

startup.ConfigureServices(services);

var serviceProvider = services.BuildServiceProvider();

var dataService = serviceProvider.GetService<DataService>()
                  ?? throw new ArgumentNullException($"{nameof(DataService)} cannot be null");

await dataService.RunAsync();