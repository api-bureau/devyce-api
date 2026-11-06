namespace ApiBureau.Devyce.Api.Dtos;

/// <summary>
/// One spoken segment of a meeting transcript.
/// </summary>
public class TranscriptCueDto
{
    /// <summary>
    /// Gets or sets the segment start, as an offset from the start of the recording.
    /// </summary>
    public double Start { get; set; }

    /// <summary>
    /// Gets or sets the segment end, as an offset from the start of the recording.
    /// </summary>
    public double End { get; set; }

    public string? Speaker { get; set; }
    public string? Text { get; set; }
}