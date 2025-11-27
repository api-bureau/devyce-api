namespace ApiBureau.Devyce.Api.Interfaces;

/// <summary>
/// High-level Devyce API client that groups available endpoints.
/// </summary>
/// <remarks>
/// This interface provides a consolidated access point to all Devyce API operations,
/// organizing endpoints by resource type (calls, users, contacts, transcripts, and CRM sync details).
/// Use the respective endpoint properties to perform operations on each resource.
/// </remarks>
public interface IDevyceClient
{
    /// <summary>
    /// Gets the endpoint for call-related operations.
    /// </summary>
    CallEndpoint Calls { get; }

    /// <summary>
    /// Gets the endpoint for user-related operations.
    /// </summary>
    UserEndpoint Users { get; }

    /// <summary>
    /// Gets the endpoint for contact-related operations.
    /// </summary>
    ContactEndpoint Contacts { get; }

    /// <summary>
    /// Gets the endpoint for call transcript operations.
    /// </summary>
    TranscriptEndpoint Transcripts { get; }

    /// <summary>
    /// Gets the endpoint for CRM synchronization details operations.
    /// </summary>
    CrmSyncDetailsEndpoint CrmSyncDetails { get; }
}