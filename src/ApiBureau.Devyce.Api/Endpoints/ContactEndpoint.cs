using ApiBureau.Devyce.Api.Responses;

namespace ApiBureau.Devyce.Api.Endpoints;

/// <summary>
/// Provides operations related to contacts within an organization.
/// </summary>
public class ContactEndpoint : BaseEndpoint
{
    /// <summary>
    /// Creates a new <see cref="ContactEndpoint"/>.
    /// </summary>
    /// <param name="httpClient">The configured Devyce HTTP connection.</param>
    public ContactEndpoint(DevyceHttpClient httpClient) : base(httpClient) { }

    /// <summary>
    /// Retrieves all contact IDs for a specific organization.
    /// </summary>
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