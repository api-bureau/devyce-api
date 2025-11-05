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
        var startDate = DateTime.Now.AddMinutes(-120);

        await FetchUsersAsync();

        var result = await FetchCallsAsync(startDate);

        await FetchCrmSyncDetailsAsync(result);

        await FetchTranscriptsAsync(result?.FirstOrDefault()?.Id);
    }

    private async Task FetchUsersAsync()
    {
        var users = await _client.Users.GetAsync(default);

        _logger.LogInformation(JsonSerializer.Serialize(users, _indentedJsonOptions));
    }

    private async Task<List<Dtos.CallDto>> FetchCallsAsync(DateTime startDate)
    {
        var callQuery = new CallQuery(startDate, DateTime.Now);

        var result = await _client.Calls.GetAsync(callQuery);

        _logger.LogInformation(JsonSerializer.Serialize(result, _indentedJsonOptions));

        return result;
    }

    private async Task FetchCrmSyncDetailsAsync(List<Dtos.CallDto> result)
    {
        foreach (var callDto in result)
        {
            var sync = await _client.CrmSyncDetails.GetAsync(callDto.Id);

            _logger.LogInformation(callDto.Id + ":" + JsonSerializer.Serialize(sync, _indentedJsonOptions));
        }
    }

    // Transcript test, you might need to request additional permission to access this endpoint
    private async Task FetchTranscriptsAsync(string? callId)
    {
        if (string.IsNullOrEmpty(callId))
        {
            _logger.LogWarning("No call ID available to fetch transcript.");

            return;
        }

        var transcript = await _client.Transcripts.GetAsync(callId, default);

        _logger.LogInformation(JsonSerializer.Serialize(transcript, _indentedJsonOptions));
    }
}