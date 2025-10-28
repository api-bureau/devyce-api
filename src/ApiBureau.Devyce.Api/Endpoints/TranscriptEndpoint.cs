namespace ApiBureau.Devyce.Api.Endpoints;

/// <summary>
/// Provides operations related to transcripts within an organization.
/// </summary>
public class TranscriptEndpoint : BaseEndpoint
{
    /// <summary>
    /// Creates a new <see cref="TranscriptEndpoint"/>.
    /// </summary>
    /// <param name="httpClient">The configured Devyce HTTP connection.</param>
    public TranscriptEndpoint(DevyceHttpClient httpClient) : base(httpClient) { }

    /// <summary>
    /// Retrieves the transcription for a specific call.
    /// </summary>
    /// <param name="callId">The call identifier.</param>
    /// <param name="cancellationToken">A token to observe for cancellation.</param>
    public async Task<CallTranscriptionDto?> GetAsync(string callId, CancellationToken cancellationToken)
        => await HttpClient.GetAsync<CallTranscriptionDto>($"/CallTranscriptions/{callId}", cancellationToken);
}