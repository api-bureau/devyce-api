namespace ApiBureau.Devyce.Api.Responses;

public class MeetingResponse
{
    public List<MeetingDto> Items { get; set; } = [];
    public string? ContinuationToken { get; set; }
}