namespace ApiBureau.Devyce.Api.Endpoints;

/// <summary>
/// Base class for Devyce API endpoint wrappers providing access to the shared <see cref="DevyceHttpClient"/>.
/// </summary>
/// <remarks>
/// This class provides the foundation for all endpoint implementations, managing the HTTP client connection
/// used for API communication. Derived endpoint classes should implement specific operations for their respective resources.
/// </remarks>
public class BaseEndpoint
{
    /// <summary>
    /// Gets the low-level HTTP connection used by derived endpoints.
    /// </summary>
    protected DevyceHttpClient HttpClient { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="BaseEndpoint"/> class.
    /// </summary>
    /// <param name="httpClient">The configured Devyce HTTP client connection.</param>
    public BaseEndpoint(DevyceHttpClient httpClient) => HttpClient = httpClient;
}