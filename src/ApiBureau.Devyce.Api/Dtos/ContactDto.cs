namespace ApiBureau.Devyce.Api.Dtos;

public class ContactDto
{
    public required string Id { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Company { get; set; }
    public List<PhoneNumberDto> PhoneNumbers { get; set; } = [];
    public string? Notes { get; set; }
    public List<string> Labels { get; set; } = [];
}