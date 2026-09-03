namespace ApiBureau.Devyce.Api.Endpoints;

/// <summary>
/// Provides operations related to users within an organization.
/// </summary>
public sealed class UserEndpoint
{
    private const string ResourcePath = "/Users";
    private readonly DevyceHttpClient _httpClient;

    /// <summary>
    /// Initializes a new instance of the <see cref="UserEndpoint"/> class.
    /// </summary>
    /// <param name="httpClient">The configured Devyce HTTP client connection.</param>
    internal UserEndpoint(DevyceHttpClient httpClient)
        => _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));

    /// <summary>
    /// Retrieves all users for a specific organization.
    /// </summary>
    /// <param name="cancellationToken">A token to observe for cancellation.</param>
    /// <returns>A list of user DTOs; an empty list if no users are found.</returns>
    public async Task<IReadOnlyList<UserDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _httpClient.GetAsync<List<UserDto>>(
            ResourcePath,
            cancellationToken).ConfigureAwait(false) ?? [];
    }
}