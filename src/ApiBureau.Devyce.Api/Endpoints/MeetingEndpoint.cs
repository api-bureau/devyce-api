using System.Net;

namespace ApiBureau.Devyce.Api.Endpoints;

/// <summary>
/// Provides operations related to meetings within an organization.
/// </summary>
public sealed class MeetingEndpoint
{
    private const string ResourcePath = "/Meetings";
    private readonly DevyceHttpClient _httpClient;

    /// <summary>
    /// Initializes a new instance of the <see cref="MeetingEndpoint"/> class.
    /// </summary>
    /// <param name="httpClient">The configured Devyce HTTP client connection.</param>
    internal MeetingEndpoint(DevyceHttpClient httpClient)
        => _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));

    /// <summary>
    /// Retrieves a single page of meetings for the configured organization.
    /// </summary>
    /// <param name="meetingQuery">The query parameters specifying the date range and pagination options.</param>
    /// <param name="cancellationToken">A token to observe for cancellation.</param>
    /// <returns>The meetings and the continuation token for the next page; an empty response if no meetings are found.</returns>
    public async Task<MeetingResponse> GetPageAsync(MeetingQuery meetingQuery,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(meetingQuery);

        var queryParams = QueryBuilder.BuildMeetingQuery(ResourcePath, meetingQuery);

        return await _httpClient.GetAsync<MeetingResponse>(queryParams, cancellationToken).ConfigureAwait(false) ?? new();
    }

    /// <summary>
    /// Retrieves all meetings for the configured organization, automatically handling pagination via continuation tokens.
    /// </summary>
    /// <param name="meetingQuery">The initial meeting query parameters specifying the date range.</param>
    /// <param name="cancellationToken">A token to observe for cancellation.</param>
    /// <returns>A list containing all meeting DTOs across all pages.</returns>
    public async Task<IReadOnlyList<MeetingDto>> GetAllAsync(MeetingQuery meetingQuery,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(meetingQuery);

        var allMeetings = new List<MeetingDto>();
        var currentQuery = meetingQuery;

        while (true)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var response = await GetPageAsync(currentQuery, cancellationToken).ConfigureAwait(false);

            allMeetings.AddRange(response.Items);

            if (string.IsNullOrEmpty(response.ContinuationToken))
            {
                break;
            }

            currentQuery = currentQuery with { ContinuationToken = response.ContinuationToken };
        }

        return allMeetings;
    }

    /// <summary>
    /// Retrieves a specific meeting by ID, including its participants and transcript.
    /// </summary>
    /// <param name="meetingId">The meeting identifier.</param>
    /// <param name="cancellationToken">A token to observe for cancellation.</param>
    /// <returns>The meeting if found; otherwise, null.</returns>
    public async Task<MeetingDto?> GetByIdAsync(string meetingId, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(meetingId);

        try
        {
            return await _httpClient.GetAsync<MeetingDto>(
                $"{ResourcePath}/{Uri.EscapeDataString(meetingId)}",
                cancellationToken).ConfigureAwait(false);
        }
        catch (HttpRequestException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }
    }
}