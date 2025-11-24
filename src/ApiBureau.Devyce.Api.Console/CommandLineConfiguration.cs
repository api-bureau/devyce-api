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
        var usersCommand = CreateUsersCommand();
        var callsCommand = CreateCallsCommand();
        var crmDetailsCommand = CreateCrmDetailsCommand();

        return new RootCommand("Devyce API Console - Interact with Devyce API to manage users, calls, and CRM data")
        {
            usersCommand,
            callsCommand,
            crmDetailsCommand
        };
    }

    private Command CreateUsersCommand()
    {
        var command = new Command("users", "List all Devyce users with their details");

        var formatOption = CreateFormatOption();
        command.Options.Add(formatOption);

        command.SetAction(async (parseResult, cancellationToken) =>
        {
            using var scope = serviceProvider.CreateScope();
            var dataService = scope.ServiceProvider.GetRequiredService<DataService>();

            var outputFormat = parseResult.GetValue(formatOption);

            await dataService.FetchAndLogUsersAsync(outputFormat);
        });

        return command;
    }

    private Command CreateCallsCommand()
    {
        var command = new Command("calls", "List Devyce calls within a specified time range");

        var formatOption = CreateFormatOption();
        var timeRangeOption = CreateTimeRangeOption();

        command.Options.Add(formatOption);
        command.Options.Add(timeRangeOption);

        command.SetAction(async (parseResult, cancellationToken) =>
        {
            using var scope = serviceProvider.CreateScope();
            var dataService = scope.ServiceProvider.GetRequiredService<DataService>();

            var outputFormat = parseResult.GetValue(formatOption);
            var lastMinutes = parseResult.GetValue(timeRangeOption);

            await dataService.FetchAndLogRecentCallsAsync(lastMinutes, outputFormat);
        });

        return command;
    }

    private Command CreateCrmDetailsCommand()
    {
        var command = new Command("crm-details", "List CRM synchronization details for Devyce calls");

        var formatOption = CreateFormatOption();
        var timeRangeOption = CreateTimeRangeOption();

        command.Options.Add(formatOption);
        command.Options.Add(timeRangeOption);

        command.SetAction(async (parseResult, cancellationToken) =>
        {
            using var scope = serviceProvider.CreateScope();
            var dataService = scope.ServiceProvider.GetRequiredService<DataService>();

            var outputFormat = parseResult.GetValue(formatOption);
            var lastMinutes = parseResult.GetValue(timeRangeOption);

            await dataService.FetchAndLogRecentCallsCrmDetailsAsync(lastMinutes, outputFormat);
        });

        return command;
    }

    private static Option<OutputFormat> CreateFormatOption()
    {
        return new Option<OutputFormat>("--format", "-f")
        {
            Description = "Output format for the results",
            DefaultValueFactory = _ => OutputFormat.Text
        };
    }

    private static Option<int> CreateTimeRangeOption()
    {
        return new Option<int>("--last-minutes", "-m")
        {
            Description = "Number of minutes to look back for calls (default: 120)",
            DefaultValueFactory = _ => 120
        };
    }
}