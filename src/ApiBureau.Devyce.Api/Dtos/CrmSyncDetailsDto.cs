namespace ApiBureau.Devyce.Api.Dtos;

public class CrmSyncDetailsDto
{
    public string CrmName { get; set; } = null!;
    public string? ContactId { get; set; }
    public string? ContactType { get; set; }
}