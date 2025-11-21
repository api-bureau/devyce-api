namespace ApiBureau.Devyce.Api.Dtos;

public class CrmSyncDetailsDto
{
    public string CrmName { get; set; } = null!;
    public string? ContactId { get; set; }
    public string? ContactType { get; set; }

    /// <summary>
    /// Gets the Bullhorn entity type corresponding to the current contact type.
    /// </summary>
    /// <remarks>Returns "ClientContact" if the contact type is "Contact"; otherwise, returns the value of the
    /// contact type. This property is useful for mapping internal contact types to Bullhorn API entity names.</remarks>
    public string? BullhornEntity => ContactType == "Contact" ? "ClientContact" : ContactType;
}