namespace ApiBureau.Devyce.Api.Dtos;

/// <summary>
/// A meeting participant: a Devyce user (with <see cref="UserId"/>) or an external attendee, with the CRM contacts they matched.
/// </summary>
public class MeetingParticipantDto
{
    public string? UserId { get; set; }
    public string? Name { get; set; }
    public string? EmailAddress { get; set; }

    /// <summary>
    /// Gets or sets the CRM contacts matched to this participant. The API's <c>MeetingCrmContact</c> has the same fields as the call sync details.
    /// </summary>
    public List<CrmSyncDetailsDto> CrmContacts { get; set; } = [];
}