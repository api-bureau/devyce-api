namespace ApiBureau.Devyce.Api.Endpoints;

/// <summary>
/// Provides operations related to contacts within an organization.
/// </summary>
public class ContactEndpoint : BaseEndpoint
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ContactEndpoint"/> class.
    /// </summary>
    /// <param name="httpClient">The configured Devyce HTTP client connection.</param>
    public ContactEndpoint(DevyceHttpClient httpClient) : base(httpClient) { }

    /// <summary>
    /// Retrieves all contact IDs for a specific organization.
    /// </summary>
    /// <param name="organizationId">The organization identifier.</param>
    /// <param name="cancellationToken">A token to observe for cancellation.</param>
    /// <returns>A contact response containing contact identifiers; null if the operation fails or no contacts are found.</returns>
    public async Task<ContactResponse?> GetContactIdsAsync(
        string organizationId,
        CancellationToken cancellationToken = default)
    {
        return await HttpClient.GetAsync<ContactResponse>(
            $"/Organizations/{organizationId}/Contacts",
            cancellationToken);
    }

    /// <summary>
    /// Retrieves a specific contact by ID.
    /// </summary>
    /// <param name="organizationId">The organization identifier.</param>
    /// <param name="contactId">The contact identifier.</param>
    /// <param name="cancellationToken">A token to observe for cancellation.</param>
    /// <returns>The contact DTO if found; otherwise, null.</returns>
    public async Task<ContactDto?> GetContactAsync(
        string organizationId,
        string contactId,
        CancellationToken cancellationToken = default)
    {
        return await HttpClient.GetAsync<ContactDto>(
            $"/Organizations/{organizationId}/Contacts/{contactId}",
            cancellationToken);
    }
}