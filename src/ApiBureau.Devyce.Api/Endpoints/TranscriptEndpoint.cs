using System.Net;

namespace ApiBureau.Devyce.Api.Endpoints;

/// <summary>
/// Provides operations related to transcripts within an organization.
/// </summary>
public sealed class TranscriptEndpoint
{
    private const string ResourcePath = "/CallTranscriptions";
    private readonly DevyceHttpClient _httpClient;

    /// <summary>
    /// Initializes a new instance of the <see cref="TranscriptEndpoint"/> class.
    /// </summary>
    /// <param name="httpClient">The configured Devyce HTTP client connection.</param>
    internal TranscriptEndpoint(DevyceHttpClient httpClient)
        => _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));

    /// <summary>
    /// Retrieves the transcription for a specific call.
    /// </summary>
    /// <param name="callId">The call identifier.</param>
    /// <param name="cancellationToken">A token to observe for cancellation.</param>
    /// <returns>The call transcription if available; otherwise, null if not found or unavailable.</returns>
    public async Task<CallTranscriptionDto?> GetForCallAsync(
        string callId,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(callId);

        try
        {
            return await _httpClient.GetAsync<CallTranscriptionDto>(
                $"{ResourcePath}/{Uri.EscapeDataString(callId)}",
                cancellationToken).ConfigureAwait(false);
        }
        catch (HttpRequestException ex) when (ex.StatusCode == HttpStatusCode.NotFound || ex.Message.Contains("404") || ex.Data.Contains("StatusCode") && ex.Data["StatusCode"]?.ToString() == "404")
        {
            return null;
        }
    }
}