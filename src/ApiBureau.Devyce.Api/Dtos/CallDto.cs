namespace ApiBureau.Devyce.Api.Dtos;

public class CallDto
{
    public string Id { get; set; } = null!;
    public string CallerId { get; set; } = null!;
    public string CalledNumber { get; set; } = null!;
    public string OriginatingNumber { get; set; } = null!;
    public string Direction { get; set; } = null!;
    public DateTime StartTimeUtc { get; set; }
    public DateTime EndTimeUtc { get; set; }
    public int Duration { get; set; }
    public string Status { get; set; } = null!;
    public DateTime UpdateTimeUtc { get; set; }
    public string? RecordingUrl { get; set; }
    public string? PlatformLink { get; set; }
    public string? CallNotes { get; set; }
    public string? CallTag { get; set; }
}