namespace ApiBureau.Devyce.Api.Endpoints;

/// <summary>
/// Provides operations related to contacts within an organization.
/// </summary>
public class ContactEndpoint : BaseEndpoint
{
    private const string BaseUrl = "/Contacts";

    /// <summary>
    /// Initializes a new instance of the <see cref="ContactEndpoint"/> class.
    /// </summary>
    /// <param name="httpClient">The configured Devyce HTTP client connection.</param>
    public ContactEndpoint(DevyceHttpClient httpClient) : base(httpClient) { }

    /// <summary>
    /// Retrieves all contact IDs for the configured organization.
    /// </summary>
    /// <param name="cancellationToken">A token to observe for cancellation.</param>
    /// <returns>A contact response containing contact identifiers; null if the operation fails or no contacts are found.</returns>
    public async Task<ContactResponse?> GetContactIdsAsync(CancellationToken cancellationToken = default)
    {
        return await HttpClient.GetAsync<ContactResponse>(BaseUrl, cancellationToken);
    }

    /// <summary>
    /// Retrieves a specific contact by ID.
    /// </summary>
    /// <param name="contactId">The contact identifier.</param>
    /// <param name="cancellationToken">A token to observe for cancellation.</param>
    /// <returns>The contact DTO if found; otherwise, null.</returns>
    public async Task<ContactDto?> GetContactAsync(string contactId, CancellationToken cancellationToken = default)
    {
        return await HttpClient.GetAsync<ContactDto>($"{BaseUrl}/{contactId}", cancellationToken);
    }
}