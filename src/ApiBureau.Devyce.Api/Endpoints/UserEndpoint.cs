namespace ApiBureau.Devyce.Api.Endpoints;

/// <summary>
/// Provides operations related to users within an organization.
/// </summary>
public class UserEndpoint : BaseEndpoint
{
    /// <summary>
    /// Creates a new <see cref="UserEndpoint"/>.
    /// </summary>
    /// <param name="httpClient">The configured Devyce HTTP connection.</param>
    public UserEndpoint(DevyceHttpClient httpClient) : base(httpClient) { }

    /// <summary>
    /// Retrieves all users for a specific organization.
    /// </summary>
    public async Task<List<UserDto>> GetAsync(CancellationToken cancellationToken)
    {
        return await HttpClient.GetAsync<List<UserDto>>(
            $"/Users",
            cancellationToken) ?? [];
    }
}