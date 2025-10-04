namespace ApiBureau.Devyce.Api.Endpoints;

/// <summary>
/// Base class for Devyce API endpoint wrappers providing access to the shared <see cref="DevyceHttpClient"/>.
/// </summary>
public class BaseEndpoint
{
    /// <summary>
    /// The low-level HTTP connection used by derived endpoints.
    /// </summary>
    protected DevyceHttpClient HttpClient { get; }

    /// <summary>
    /// Initializes the endpoint with a shared <see cref="DevyceHttpClient"/> instance.
    /// </summary>
    /// <param name="httpClient">The configured Devyce HTTP connection.</param>
    public BaseEndpoint(DevyceHttpClient httpClient) => HttpClient = httpClient;
}