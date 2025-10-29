namespace ApiBureau.Devyce.Api;

/// <summary>
/// Default implementation of <see cref="IDevyceClient"/> that exposes grouped Devyce endpoints.
/// </summary>
public class DevyceClient : IDevyceClient
{
    /// <summary>
    /// Operations related to call records and call history.
    /// </summary>
    public CallEndpoint Calls { get; }
    public ContactEndpoint Contacts { get; }
    public UserEndpoint Users { get; }

    public TranscriptEndpoint Transcripts { get; }

    public CrmSyncDetailsEndpoint CrmSyncDetails { get; }

    /// <summary>
    /// Creates a new <see cref="DevyceClient"/> instance.
    /// </summary>
    /// <param name="apiConnection">The configured Devyce HTTP connection.</param>
    public DevyceClient(DevyceHttpClient apiConnection)
    {
        Calls = new CallEndpoint(apiConnection);
        Contacts = new ContactEndpoint(apiConnection);
        Users = new UserEndpoint(apiConnection);
        Transcripts = new TranscriptEndpoint(apiConnection);
        CrmSyncDetails = new CrmSyncDetailsEndpoint(apiConnection);
    }
}