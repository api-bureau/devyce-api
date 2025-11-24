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
        var callsCommand = CreateCallsCommand(exportPathArgument);

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
        var userCommand = new Command("users", "Fetch and list all Devyce users");

        var jsonOption = new Option<bool>("--json", "-j")
        {
            Description = "Output users as JSON format"
        };

        userCommand.Options.Add(jsonOption);

        userCommand.SetAction(async (parseResult, cancellationToken) =>
        {
            using var scope = serviceProvider.CreateScope();
            var dataService = scope.ServiceProvider.GetRequiredService<DataService>();

            var useJson = parseResult.GetValue(jsonOption);

            if (useJson)
            {
                await dataService.FetchAndLogUsersAsJsonAsync();
            }
            else
            {
                await dataService.FetchAndLogUsersAsync();
            }
        });

        return userCommand;
    }

    private Command CreateCallsCommand(Argument<DirectoryInfo> pathArgument)
    {
        var callsCommand = new Command("calls", "Fetch and list Devyce calls");

        var jsonOption = new Option<bool>("--json", "-j")
        {
            Description = "Output calls as JSON format"
        };

        var lastMinutesOption = new Option<int>("--last-minutes", "-l")
        {
            Description = "Last x minutes",
            DefaultValueFactory = parsedValue => 120
        };

        callsCommand.Options.Add(jsonOption);
        callsCommand.Options.Add(lastMinutesOption);
        callsCommand.Arguments.Add(pathArgument);

        callsCommand.SetAction(async (parseResult, cancellationToken) =>
        {
            using var scope = serviceProvider.CreateScope();
            var dataService = scope.ServiceProvider.GetRequiredService<DataService>();
            var path = parseResult.GetValue(pathArgument);

            var useLastMinuteOptions = parseResult.GetValue(lastMinutesOption);

            var useJson = parseResult.GetValue(jsonOption);

            if (useJson)
            {
                await dataService.FetchAndLogRecentCallsAsJsonAsync(useLastMinuteOptions);
            }
            else
            {
                await dataService.FetchAndLogRecentCallsAsync(useLastMinuteOptions);
            }
        });

        return callsCommand;
    }
}