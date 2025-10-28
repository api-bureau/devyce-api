namespace ApiBureau.Devyce.Api.Endpoints;

/// <summary>
/// Provides operations related to calls within an organization.
/// </summary>
public class CallEndpoint : BaseEndpoint
{
    /// <summary>
    /// Creates a new <see cref="CallEndpoint"/>.
    /// </summary>
    /// <param name="httpClient">The configured Devyce HTTP connection.</param>
    public CallEndpoint(DevyceHttpClient httpClient) : base(httpClient) { }

    /// <summary>
    /// Retrieves calls for a specific organization with pagination support.
    /// </summary>
    /// <param name="cancellationToken">A token to observe for cancellation.</param>
    public async Task<List<CallDto>> GetAsync(CallQuery callQuery,
        CancellationToken cancellationToken = default)
    {
        var queryParams = QueryBuilder.BuildCallQuery(callQuery);

        var response = await HttpClient.GetAsync<CallResponse>(queryParams,
            cancellationToken);

        return response?.Items ?? [];
    }
}