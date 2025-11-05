namespace ApiBureau.Devyce.Api.Endpoints;

/// <summary>
/// Provides operations related to CRM sync details for calls.
/// </summary>
public class CrmSyncDetailsEndpoint : BaseEndpoint
{
    /// <summary>
    /// Creates a new <see cref="CrmSyncDetailsEndpoint"/>.
    /// </summary>
    /// <param name="httpClient">The configured Devyce HTTP connection.</param>
    public CrmSyncDetailsEndpoint(DevyceHttpClient httpClient) : base(httpClient) { }

    /// <summary>
    /// Retrieves sync details for a specific call for a specific organization.
    /// </summary>
    /// <param name="cancellationToken">A token to observe for cancellation.</param>
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