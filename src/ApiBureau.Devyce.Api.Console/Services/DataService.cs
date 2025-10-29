using ApiBureau.Devyce.Api.Interfaces;
using ApiBureau.Devyce.Api.Queries;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace ApiBureau.Devyce.Api.Console.Services;

public class DataService
{
    private readonly IDevyceClient _client;
    private readonly ILogger<DataService> _logger;
    private readonly JsonSerializerOptions _indentedJsonOptions = new() { WriteIndented = true };

    public DataService(IDevyceClient client, ILogger<DataService> logger)
    {
        _client = client;
        _logger = logger;
    }

    public async Task RunAsync()
    {
        var callQuery = new CallQuery(DateTime.Now.AddDays(-1), DateTime.Now);

        var result = await _client.Calls.GetAsync(callQuery);

        _logger.LogInformation(JsonSerializer.Serialize(result, _indentedJsonOptions));

        var users = await _client.Users.GetAsync(default);

        _logger.LogInformation(JsonSerializer.Serialize(users, _indentedJsonOptions));

        foreach (var callDto in result)
        {
            var sync = await _client.CrmSyncDetails.GetAsync(callDto.Id);
            _logger.LogInformation(callDto.Id + ":" + JsonSerializer.Serialize(sync, _indentedJsonOptions));
        }

        // Transcript test, you might need to request additional permission to access this endpoint

        //var transcript = await _client.Transcripts.GetAsync("", default);

        //_logger.LogInformation(JsonSerializer.Serialize(transcript, _indentedJsonOptions));
    }
}