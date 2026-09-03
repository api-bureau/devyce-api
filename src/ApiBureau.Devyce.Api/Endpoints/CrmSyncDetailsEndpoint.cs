using System.Net;

namespace ApiBureau.Devyce.Api.Endpoints;

/// <summary>
/// Provides operations related to CRM sync details for calls.
/// </summary>
public sealed class CrmSyncDetailsEndpoint
{
    private const string ResourcePath = "/Calls";
    private readonly DevyceHttpClient _httpClient;

    /// <summary>
    /// Initializes a new instance of the <see cref="CrmSyncDetailsEndpoint"/> class.
    /// </summary>
    /// <param name="httpClient">The configured Devyce HTTP client connection.</param>
    internal CrmSyncDetailsEndpoint(DevyceHttpClient httpClient)
        => _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));

    /// <summary>
    /// Retrieves sync details for a specific call within an organization.
    /// </summary>
    /// <param name="callId">The call identifier.</param>
    /// <param name="cancellationToken">A token to observe for cancellation.</param>
    /// <returns>A collection of CRM sync details DTOs; an empty list if no sync details are found or unavailable.</returns>
    public async Task<IReadOnlyList<CrmSyncDetailsDto>> GetForCallAsync(string callId,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(callId);

        try
        {
            return await _httpClient.GetAsync<List<CrmSyncDetailsDto>>(
                $"{ResourcePath}/{Uri.EscapeDataString(callId)}/SyncDetails",
                cancellationToken).ConfigureAwait(false) ?? [];
        }
        catch (HttpRequestException ex) when (ex.StatusCode == HttpStatusCode.NotFound || ex.Message.Contains("404") || ex.Data.Contains("StatusCode") && ex.Data["StatusCode"]?.ToString() == "404")
        {
            return [];
        }
    }
}