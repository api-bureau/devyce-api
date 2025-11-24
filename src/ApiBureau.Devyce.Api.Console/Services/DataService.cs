using ApiBureau.Devyce.Api.Dtos;
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
        await FetchAndLogUsersAsJsonAsync();

        var result = await FetchAndLogRecentCallsAsJsonAsync();

        await FetchCrmSyncDetailsAsync(result);

        await FetchTranscriptsAsync(result?.FirstOrDefault()?.Id);
    }

    public async Task FetchAndLogUsersAsJsonAsync()
    {
        var users = await FetchUsersAsync();

        _logger.LogInformation("** List Devyce users as JSON ***");
        _logger.LogInformation(JsonSerializer.Serialize(users, _indentedJsonOptions));
        _logger.LogInformation("Total users: {count}", users.Count);
    }

    public async Task FetchAndLogUsersAsync()
    {
        var users = await FetchUsersAsync();

        _logger.LogInformation("** List Devyce users ***");

        foreach (var user in users.OrderByDescending(s => s.ActiveState).ThenBy(s => s.FullName))
        {
            _logger.LogInformation("{active}: {name}: {email}", user.ActiveState, user.FullName, user.EmailAddress);
        }

        _logger.LogInformation("Total users: {count}", users.Count);
    }

    public async Task<List<CallDto>> FetchAndLogRecentCallsAsJsonAsync(int minutes = 120)
    {
        var startDate = DateTime.Now.AddMinutes(-minutes);

        var calls = await FetchCallsAsync(startDate);

        _logger.LogInformation("** List Devyce calls as JSON ***");
        _logger.LogInformation(JsonSerializer.Serialize(calls, _indentedJsonOptions));
        _logger.LogInformation("Total calls: {count}", calls.Count);

        return calls;
    }

    public async Task<List<CallDto>> FetchAndLogRecentCallsAsync(int minutes = 120)
    {
        var startDate = DateTime.Now.AddMinutes(-minutes);

        var calls = await FetchCallsAsync(startDate);

        _logger.LogInformation("** List Devyce calls ***");

        foreach (var call in calls)
        {
            _logger.LogInformation("Start time: {start}, duration: {duration}, caller: {callerNumber}, called: {calledNumber}", call.StartTimeUtc, call.Duration, call.OriginatingNumber, call.CalledNumber);
        }

        _logger.LogInformation("Total calls: {count}", calls.Count);

        return calls;
    }

    public async Task<List<(string CallId, CrmSyncDetailsDto Detail)>> FetchAndLogRecentCallsCrmDetailsAsJsonAsync(int minutes = 120)
    {
        var startDate = DateTime.Now.AddMinutes(-minutes);

        var calls = await FetchCallsAsync(startDate);
        var crmDetails = await FetchCrmSyncDetailsAsync(calls, logEachItem: false);

        _logger.LogInformation("** List Devyce CRM call details as JSON ***");
        foreach (var item in crmDetails)
        {
            _logger.LogInformation("CallId: {id}, crm: {crm}", item.CallId, JsonSerializer.Serialize(item.Detail, _indentedJsonOptions));
        }
        _logger.LogInformation("Total details: {count}", crmDetails.Count);

        return crmDetails;
    }

    public async Task<List<CallDto>> FetchAndLogRecentCallsCrmDetailsAsync(int minutes = 120)
    {
        var startDate = DateTime.Now.AddMinutes(-minutes);

        var calls = await FetchCallsAsync(startDate);
        var crmDetails = await FetchCrmSyncDetailsAsync(calls, logEachItem: false);

        _logger.LogInformation("** List Devyce calls ***");

        foreach (var call in calls)
        {
            var crmDetail = crmDetails.Where(w => w.CallId == call.Id).Select(s => $"{s.Detail.CrmName}:{s.Detail.ContactType}:{s.Detail.ContactId}").ToList();
            var joinCrmDetails = string.Join(", ", crmDetail);

            _logger.LogInformation("Start time: {start}, duration: {duration}, caller: {callerNumber}, called: {calledNumber}, crm: {crm}", call.StartTimeUtc, call.Duration, call.OriginatingNumber, call.CalledNumber, joinCrmDetails);
        }

        _logger.LogInformation("Total calls: {count}", calls.Count);

        return calls;
    }

    private async Task<List<UserDto>> FetchUsersAsync()
    {
        _logger.LogInformation("** Fetching Devyce users ***");

        var users = await _client.Users.GetAsync(default);

        _logger.LogInformation("Fetched users: {count}", users.Count);

        return users;
    }

    private async Task<List<CallDto>> FetchCallsAsync(DateTime startDate)
    {
        var callQuery = new CallQuery(startDate, DateTime.Now);

        _logger.LogInformation("** Fetching Devyce calls ***");

        var calls = await _client.Calls.GetAsync(callQuery);

        _logger.LogInformation("Fetched calls: {count}", calls.Count);

        return calls;
    }

    private async Task<List<(string CallId, CrmSyncDetailsDto Detail)>> FetchCrmSyncDetailsAsync(List<CallDto> calls, bool logEachItem = true)
    {
        var crmDetails = new List<(string CallId, CrmSyncDetailsDto Detail)>();

        _logger.LogInformation("** Fetching Devyce CRM details ***");

        foreach (var callDto in calls)
        {
            var crmSyncDetails = await _client.CrmSyncDetails.GetAsync(callDto.Id);

            if (logEachItem)
            {
                _logger.LogInformation(callDto.Id + ":" + JsonSerializer.Serialize(crmSyncDetails, _indentedJsonOptions));
            }

            if (crmSyncDetails is null) continue;

            foreach (var detail in crmSyncDetails.Where(w => !string.IsNullOrWhiteSpace(w.ContactId)))
            {
                crmDetails.Add((callDto.Id, detail));
            }
        }

        _logger.LogInformation("Fetched CRM details from {calls} calls: {count}", calls.Count, crmDetails.Count);

        return crmDetails;
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