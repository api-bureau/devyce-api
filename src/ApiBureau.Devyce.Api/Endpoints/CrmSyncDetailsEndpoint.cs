namespace ApiBureau.Devyce.Api.Endpoints;

/// <summary>
/// Provides operations related to CRM sync details for calls.
/// </summary>
public class CrmSyncDetailsEndpoint : BaseEndpoint
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CrmSyncDetailsEndpoint"/> class.
    /// </summary>
    /// <param name="httpClient">The configured Devyce HTTP client connection.</param>
    public CrmSyncDetailsEndpoint(DevyceHttpClient httpClient) : base(httpClient) { }

    /// <summary>
    /// Retrieves sync details for a specific call within an organization.
    /// </summary>
    /// <param name="callId">The call identifier.</param>
    /// <param name="cancellationToken">A token to observe for cancellation.</param>
    /// <returns>A collection of CRM sync details DTOs; an empty list if no sync details are found or unavailable.</returns>
    public async Task<IList<CrmSyncDetailsDto>> GetAsync(string callId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            return await HttpClient.GetAsync<IList<CrmSyncDetailsDto>>($"/Calls/{callId}/SyncDetails", cancellationToken) ?? [];
        }
        catch (HttpRequestException ex) when (ex.Message.Contains("404") || ex.Data.Contains("StatusCode") && ex.Data["StatusCode"]?.ToString() == "404")
        {
            return [];
        }
    }
}