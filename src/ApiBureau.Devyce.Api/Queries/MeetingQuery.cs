namespace ApiBureau.Devyce.Api.Queries;

public sealed record MeetingQuery(
    DateTime? StartDateInclusive,
    DateTime? EndDateExclusive,
    int? Limit = null,
    string? ContinuationToken = null);