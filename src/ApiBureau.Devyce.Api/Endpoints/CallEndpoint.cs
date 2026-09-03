namespace ApiBureau.Devyce.Api.Endpoints;

/// <summary>
/// Provides operations related to calls within an organization.
/// </summary>
public sealed class CallEndpoint
{
    private const string ResourcePath = "/Calls";
    private readonly DevyceHttpClient _httpClient;

    /// <summary>
    /// Initializes a new instance of the <see cref="CallEndpoint"/> class.
    /// </summary>
    /// <param name="httpClient">The configured Devyce HTTP client connection.</param>
    internal CallEndpoint(DevyceHttpClient httpClient)
        => _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));

    /// <summary>
    /// Retrieves a single page of calls for a specific organization.
    /// </summary>
    /// <param name="callQuery">The query parameters specifying the call filter criteria and pagination options.</param>
    /// <param name="cancellationToken">A token to observe for cancellation.</param>
    /// <returns>A list of call DTOs for the requested page; an empty list if no calls are found.</returns>
    public async Task<IReadOnlyList<CallDto>> GetPageAsync(CallQuery callQuery,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(callQuery);

        var queryParams = QueryBuilder.BuildCallQuery(ResourcePath, callQuery);

        var response = await _httpClient.GetAsync<CallResponse>(queryParams, cancellationToken).ConfigureAwait(false);

        return response?.Items ?? [];
    }

    /// <summary>
    /// Retrieves all calls for a specific organization, automatically handling pagination via continuation tokens.
    /// </summary>
    /// <remarks>
    /// This method handles pagination transparently by following continuation tokens returned by the API
    /// until all available results are retrieved. It is suitable for scenarios where all call data is required.
    /// </remarks>
    /// <param name="callQuery">The initial call query parameters specifying the filter criteria.</param>
    /// <param name="cancellationToken">A token to observe for cancellation.</param>
    /// <returns>A list containing all call DTOs across all pages.</returns>
    public async Task<IReadOnlyList<CallDto>> GetAllAsync(CallQuery callQuery,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(callQuery);

        var allCalls = new List<CallDto>();
        var currentQuery = callQuery;

        while (true)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var queryParams = QueryBuilder.BuildCallQuery(ResourcePath, currentQuery);
            var response = await _httpClient.GetAsync<CallResponse>(queryParams, cancellationToken).ConfigureAwait(false);

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