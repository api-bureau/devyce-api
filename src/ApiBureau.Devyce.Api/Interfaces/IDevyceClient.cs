namespace ApiBureau.Devyce.Api.Interfaces;

/// <summary>
/// High-level Devyce API client that groups available endpoints.
/// </summary>
public interface IDevyceClient
{
    /// <summary>
    /// Access to call-related operations.
    /// </summary>
    CallEndpoint Calls { get; }

    /// <summary>
    /// Gets the endpoint for managing user-related operations.
    /// </summary>
    UserEndpoint Users { get; }

    /// <summary>
    /// Gets the endpoint for managing contact information.
    /// </summary>
    ContactEndpoint Contacts { get; }

    /// <summary>
    /// Gets the endpoint used to access call transcripts.
    /// </summary>
    TranscriptEndpoint Transcripts { get; }
}