namespace ApiBureau.Devyce.Api;

/// <summary>
/// Default implementation of <see cref="IDevyceClient"/> that exposes grouped Devyce API endpoints.
/// </summary>
/// <remarks>
/// This class initializes all endpoint instances using the provided HTTP client connection,
/// making them available as properties for consuming code. Each endpoint is instantiated during
/// construction and cached for reuse.
/// </remarks>
public sealed class DevyceClient : IDevyceClient
{
    /// <summary>
    /// Gets the endpoint for call-related operations and call history retrieval.
    /// </summary>
    public CallEndpoint Calls { get; }

    /// <summary>
    /// Gets the endpoint for contact-related operations.
    /// </summary>
    public ContactEndpoint Contacts { get; }

    /// <summary>
    /// Gets the endpoint for user-related operations.
    /// </summary>
    public UserEndpoint Users { get; }

    /// <summary>
    /// Gets the endpoint for call transcript operations.
    /// </summary>
    public TranscriptEndpoint Transcripts { get; }

    /// <summary>
    /// Gets the endpoint for CRM synchronization details operations.
    /// </summary>
    public CrmSyncDetailsEndpoint CrmSyncDetails { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="DevyceClient"/> class.
    /// </summary>
    /// <remarks>
    /// This constructor instantiates all endpoint objects and caches them for the lifetime of the client instance.
    /// All endpoints share the same HTTP client connection for consistent authentication and configuration.
    /// </remarks>
    /// <param name="httpClient">The configured Devyce HTTP client connection used by all endpoints.</param>
    public DevyceClient(DevyceHttpClient httpClient)
    {
        ArgumentNullException.ThrowIfNull(httpClient);

        Calls = new CallEndpoint(httpClient);
        Contacts = new ContactEndpoint(httpClient);
        Users = new UserEndpoint(httpClient);
        Transcripts = new TranscriptEndpoint(httpClient);
        CrmSyncDetails = new CrmSyncDetailsEndpoint(httpClient);
    }
}