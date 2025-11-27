namespace ApiBureau.Devyce.Api.Endpoints;

/// <summary>
/// Provides operations related to users within an organization.
/// </summary>
public class UserEndpoint : BaseEndpoint
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UserEndpoint"/> class.
    /// </summary>
    /// <param name="httpClient">The configured Devyce HTTP client connection.</param>
    public UserEndpoint(DevyceHttpClient httpClient) : base(httpClient) { }

    /// <summary>
    /// Retrieves all users for a specific organization.
    /// </summary>
    /// <param name="cancellationToken">A token to observe for cancellation.</param>
    /// <returns>A list of user DTOs; an empty list if no users are found.</returns>
    public async Task<List<UserDto>> GetAsync(CancellationToken cancellationToken = default)
    {
        return await HttpClient.GetAsync<List<UserDto>>(
            $"/Users",
            cancellationToken) ?? [];
    }
}