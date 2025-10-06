using ApiBureau.Devyce.Api.Interfaces;
using ApiBureau.Devyce.Api.Queries;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace ApiBureau.Devyce.Api.Console.Services;

public class DataService
{
    private readonly IDevyceClient _client;
    private readonly ILogger<DataService> _logger;

    public DataService(IDevyceClient client, ILogger<DataService> logger)
    {
        _client = client;
        _logger = logger;
    }

    public async Task RunAsync()
    {
        var callQuery = new CallQuery(DateTime.Now.AddDays(-1), DateTime.Now);

        var result = await _client.Calls.GetAsync(callQuery);

        _logger.LogInformation(JsonSerializer.Serialize(result, new JsonSerializerOptions { WriteIndented = true }));

        var users = await _client.Users.GetAsync(default);

        _logger.LogInformation(JsonSerializer.Serialize(users, new JsonSerializerOptions { WriteIndented = true }));
    }
}