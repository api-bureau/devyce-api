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

        var response = await HttpClient.GetAsync<CallResponse>(queryParams, cancellationToken);

        return response?.Items ?? [];
    }

    /// <summary>
    /// Retrieves all calls for a specific organization, automatically handling pagination via continuation tokens.
    /// </summary>
    /// <param name="callQuery">The initial call query parameters.</param>
    /// <param name="cancellationToken">A token to observe for cancellation.</param>
    /// <returns>A list containing all call DTOs across all pages.</returns>
    public async Task<List<CallDto>> GetAllAsync(CallQuery callQuery,
        CancellationToken cancellationToken = default)
    {
        var allCalls = new List<CallDto>();
        var currentQuery = callQuery;

        while (true)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var queryParams = QueryBuilder.BuildCallQuery(currentQuery);
            var response = await HttpClient.GetAsync<CallResponse>(queryParams, cancellationToken);

            if (response?.Items is { Count: > 0 })
            {
                allCalls.AddRange(response.Items);
            }

            if (string.IsNullOrEmpty(response?.ContinuationToken))
            {
                break;
            }

            currentQuery = currentQuery with { ContinuationToken = response.ContinuationToken };
        }

        return allCalls;
    }
}