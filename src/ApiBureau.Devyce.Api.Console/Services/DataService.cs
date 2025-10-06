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
        var callQuery = new CallQuery(DateTime.Now, DateTime.Now.AddDays(-5));

        var result = await _client.Calls.GetCallsAsync(callQuery);

        _logger.LogInformation(JsonSerializer.Serialize(result, new JsonSerializerOptions { WriteIndented = true }));
    }
}