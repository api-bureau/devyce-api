namespace ApiBureau.Devyce.Api.Responses;

public class CallResponse
{
    public List<CallDto> Items { get; set; } = [];
    public string? ContinuationToken { get; set; }
}