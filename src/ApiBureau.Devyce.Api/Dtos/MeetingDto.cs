namespace ApiBureau.Devyce.Api.Dtos;

/// <summary>
/// A meeting recorded by Devyce, with its participants and transcript.
/// </summary>
public class MeetingDto
{
    public string Id { get; set; } = null!;
    public string? Source { get; set; }
    public string? Subject { get; set; }
    public DateTime StartTimeUtc { get; set; }

    /// <summary>
    /// Gets or sets the meeting duration as reported by Devyce; the API doesn't state the unit (call durations are in seconds).
    /// </summary>
    public int? Duration { get; set; }

    public DateTime UpdateTimeUtc { get; set; }
    public string? Summary { get; set; }
    public string? CallTag { get; set; }
    public List<MeetingParticipantDto> Participants { get; set; } = [];
    public List<TranscriptCueDto> Transcript { get; set; } = [];
}