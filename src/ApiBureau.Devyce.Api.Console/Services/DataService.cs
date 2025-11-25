using ApiBureau.Devyce.Api.Dtos;
using ApiBureau.Devyce.Api.Interfaces;
using ApiBureau.Devyce.Api.Queries;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace ApiBureau.Devyce.Api.Console.Services;

/// <summary>
/// Service for fetching and displaying Devyce data in various formats.
/// </summary>
public class DataService
{
    private readonly IDevyceClient _client;
    private readonly ILogger<DataService> _logger;
    private readonly JsonSerializerOptions _jsonOptions = new() { WriteIndented = true };

    private const int DefaultTimeRangeMinutes = 120;

    public DataService(IDevyceClient client, ILogger<DataService> logger)
    {
        _client = client;
        _logger = logger;
    }

    /// <summary>
    /// Fetches and logs all Devyce users in the specified format.
    /// </summary>
    public async Task FetchAndLogUsersAsync(OutputFormat format = OutputFormat.Text)
    {
        var users = await FetchUsersAsync();

        _logger.LogInformation("=== Devyce Users ===");

        if (format == OutputFormat.Json)
        {
            _logger.LogInformation(JsonSerializer.Serialize(users, _jsonOptions));
        }
        else
        {
            foreach (var user in users.OrderByDescending(u => u.ActiveState).ThenBy(u => u.FullName))
            {
                _logger.LogInformation("{Status} | {Name} | {Email}",
                    user.ActiveState,
                    user.FullName,
                    user.EmailAddress);
            }
        }

        _logger.LogInformation("Total users: {Count}", users.Count);
    }

    /// <summary>
    /// Fetches and logs recent calls within the specified time range.
    /// </summary>
    public async Task<List<CallDto>> FetchAndLogRecentCallsAsync(
        int lastMinutes = DefaultTimeRangeMinutes,
        OutputFormat format = OutputFormat.Text)
    {
        var startDate = DateTime.Now.AddMinutes(-lastMinutes);
        var calls = await FetchCallsAsync(startDate);

        _logger.LogInformation("=== Devyce Calls (Last {Minutes} minutes) ===", lastMinutes);

        if (format == OutputFormat.Json)
        {
            _logger.LogInformation(JsonSerializer.Serialize(calls, _jsonOptions));
        }
        else
        {
            foreach (var call in calls)
            {
                _logger.LogInformation("{StartTime:yyyy-MM-dd HH:mm:ss} | Duration: {Duration}s | From: {Caller} | To: {Called} | Id: {CallId}",
                    call.StartTimeUtc,
                    call.Duration,
                    call.OriginatingNumber,
                    call.CalledNumber,
                    call.Id);
            }
        }

        _logger.LogInformation("Total calls: {Count}", calls.Count);

        return calls;
    }

    /// <summary>
    /// Fetches and logs CRM synchronization details for recent calls.
    /// </summary>
    public async Task FetchAndLogRecentCallsCrmDetailsAsync(
        int lastMinutes = DefaultTimeRangeMinutes,
        OutputFormat format = OutputFormat.Text)
    {
        var startDate = DateTime.Now.AddMinutes(-lastMinutes);
        var calls = await FetchCallsAsync(startDate);
        var crmDetails = await FetchCrmSyncDetailsAsync(calls);

        _logger.LogInformation("=== Devyce CRM Sync Details (Last {Minutes} minutes) ===", lastMinutes);

        if (format == OutputFormat.Json)
        {
            var detailsForJson = crmDetails.Select(d => new
            {
                CallId = d.CallId,
                CrmDetails = d.Details
            });
            _logger.LogInformation(JsonSerializer.Serialize(detailsForJson, _jsonOptions));
        }
        else
        {
            foreach (var call in calls)
            {
                var matchingDetails = crmDetails
                    .Where(d => d.CallId == call.Id)
                    .SelectMany(d => d.Details)
                    .Select(detail => $"{detail.CrmName}:{detail.ContactType}:{detail.ContactId}")
                    .ToList();

                var crmInfo = matchingDetails.Any() ? string.Join(", ", matchingDetails) : "No CRM data";

                _logger.LogInformation("{StartTime:yyyy-MM-dd HH:mm:ss} | Duration: {Duration}s | From: {Caller} | To: {Called} | CRM: {Crm}",
                    call.StartTimeUtc,
                    call.Duration,
                    call.OriginatingNumber,
                    call.CalledNumber,
                    crmInfo);
            }
        }

        _logger.LogInformation("Total calls: {CallCount} | CRM details found: {CrmCount}",
            calls.Count,
            crmDetails.Sum(d => d.Details.Count));
    }

    private async Task<List<UserDto>> FetchUsersAsync()
    {
        _logger.LogDebug("Fetching Devyce users...");

        var users = await _client.Users.GetAsync(default);

        _logger.LogDebug("Fetched {Count} users", users.Count);

        return users;
    }

    private async Task<List<CallDto>> FetchCallsAsync(DateTime startDate)
    {
        var callQuery = new CallQuery(startDate, DateTime.Now);

        _logger.LogDebug("Fetching Devyce calls from {StartDate}...", startDate);

        var calls = await _client.Calls.GetAsync(callQuery);

        _logger.LogDebug("Fetched {Count} calls", calls.Count);

        return calls;
    }

    private async Task<List<(string CallId, List<CrmSyncDetailsDto> Details)>> FetchCrmSyncDetailsAsync(List<CallDto> calls)
    {
        var crmDetailsList = new List<(string CallId, List<CrmSyncDetailsDto> Details)>();

        _logger.LogDebug("Fetching CRM sync details for {Count} calls...", calls.Count);

        foreach (var call in calls)
        {
            var crmSyncDetails = await _client.CrmSyncDetails.GetAsync(call.Id);

            if (crmSyncDetails is null || crmSyncDetails.Count == 0)
                continue;

            var validDetails = crmSyncDetails
                .Where(detail => !string.IsNullOrWhiteSpace(detail.ContactId))
                .ToList();

            if (validDetails.Count > 0)
            {
                crmDetailsList.Add((call.Id, validDetails));
            }
        }

        _logger.LogDebug("Fetched CRM details for {Count} calls", crmDetailsList.Count);

        return crmDetailsList;
    }

    /// <summary>
    /// Fetches and logs call transcript (requires additional API permissions).
    /// </summary>
    public async Task FetchAndLogTranscriptAsync(string? callId)
    {
        if (string.IsNullOrWhiteSpace(callId))
        {
            _logger.LogWarning("No call ID provided for transcript fetch");

            return;
        }

        _logger.LogInformation("=== Transcript for Call {CallId} ===", callId);

        var transcript = await _client.Transcripts.GetAsync(callId, default);

        _logger.LogInformation("{Transcript}", JsonSerializer.Serialize(transcript, _jsonOptions));
    }
}