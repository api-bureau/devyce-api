namespace ApiBureau.Devyce.Api.Endpoints;

/// <summary>
/// Provides operations related to contacts within an organization.
/// </summary>
public sealed class ContactEndpoint
{
    private const string ResourcePath = "/Contacts";
    private readonly DevyceHttpClient _httpClient;

    /// <summary>
    /// Initializes a new instance of the <see cref="ContactEndpoint"/> class.
    /// </summary>
    /// <param name="httpClient">The configured Devyce HTTP client connection.</param>
    internal ContactEndpoint(DevyceHttpClient httpClient)
        => _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));

    /// <summary>
    /// Retrieves all contact IDs for the configured organization.
    /// </summary>
    /// <param name="cancellationToken">A token to observe for cancellation.</param>
    /// <returns>A contact response containing contact identifiers; null if the operation fails or no contacts are found.</returns>
    public Task<ContactResponse?> GetIdsAsync(CancellationToken cancellationToken = default)
        => _httpClient.GetAsync<ContactResponse>(ResourcePath, cancellationToken);

    /// <summary>
    /// Retrieves a specific contact by ID.
    /// </summary>
    /// <param name="contactId">The contact identifier.</param>
    /// <param name="cancellationToken">A token to observe for cancellation.</param>
    /// <returns>The contact DTO if found; otherwise, null.</returns>
    public Task<ContactDto?> GetByIdAsync(string contactId, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(contactId);

        return _httpClient.GetAsync<ContactDto>(
            $"{ResourcePath}/{Uri.EscapeDataString(contactId)}",
            cancellationToken);
    }
}