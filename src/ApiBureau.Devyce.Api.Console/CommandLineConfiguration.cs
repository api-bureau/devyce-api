using ApiBureau.Devyce.Api.Console.Services;
using Microsoft.Extensions.DependencyInjection;
using System.CommandLine;

namespace ApiBureau.Devyce.Api.Console;

/// <summary>
/// Builds and configures the command-line interface structure.
/// </summary>
/// <param name="serviceProvider">Service provider for resolving command handlers.</param>
public class CommandLineConfiguration(IServiceProvider serviceProvider)
{
    /// <summary>
    /// Creates and configures the root command with all subcommands.
    /// </summary>
    /// <returns>Configured root command ready for parsing.</returns>
    public RootCommand CreateRootCommand()
    {
        var exportPathArgument = CreatePathArgument();
        var usersCommand = CreateUsersCommand();
        var callsCommand = CreateGenerateCommand(exportPathArgument);

        return new RootCommand("Devyce API examples")
        {
            usersCommand,
            callsCommand
        };
    }

    private static Argument<DirectoryInfo> CreatePathArgument()
    {
        return new Argument<DirectoryInfo>("export-path")
        {
            Description = "The export path where the items will be exported",
            DefaultValueFactory = _ => new DirectoryInfo(Path.Combine("export", "data"))
        };
    }

    private Command CreateUsersCommand()
    {
        var scanCommand = new Command("users", "Fetches and lists all Devyce users");

        scanCommand.SetAction(async (parseResult, cancellationToken) =>
        {
            using var scope = serviceProvider.CreateScope();
            var dataService = scope.ServiceProvider.GetRequiredService<DataService>();

            await dataService.FetchAndLogUsersAsync();
        });

        return scanCommand;
    }

    private Command CreateGenerateCommand(Argument<DirectoryInfo> pathArgument)
    {
        var generateCommand = new Command("generate", "Generates the metadata JSON manifest.");
        generateCommand.Arguments.Add(pathArgument);
        generateCommand.SetAction(async (parseResult, cancellationToken) =>
        {
            using var scope = serviceProvider.CreateScope();
            var handler = scope.ServiceProvider.GetRequiredService<DataService>();
            var path = parseResult.GetValue(pathArgument);

            await handler.RunAsync();
        });

        return generateCommand;
    }
}