namespace ApiBureau.Devyce.Api.Endpoints;

/// <summary>
/// Provides operations related to transcripts within an organization.
/// </summary>
public class TranscriptEndpoint : BaseEndpoint
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TranscriptEndpoint"/> class.
    /// </summary>
    /// <param name="httpClient">The configured Devyce HTTP client connection.</param>
    public TranscriptEndpoint(DevyceHttpClient httpClient) : base(httpClient) { }

    /// <summary>
    /// Retrieves the transcription for a specific call.
    /// </summary>
    /// <param name="callId">The call identifier.</param>
    /// <param name="cancellationToken">A token to observe for cancellation.</param>
    /// <returns>The call transcription if available; otherwise, null if not found or unavailable.</returns>
    public async Task<CallTranscriptionDto?> GetAsync(string callId, CancellationToken cancellationToken)
    {
        try
        {
            return await HttpClient.GetAsync<CallTranscriptionDto>($"/CallTranscriptions/{callId}", cancellationToken);
        }
        catch (HttpRequestException ex) when (ex.Message.Contains("404") || ex.Data.Contains("StatusCode") && ex.Data["StatusCode"]?.ToString() == "404")
        {
            return null;
        }
    }
}