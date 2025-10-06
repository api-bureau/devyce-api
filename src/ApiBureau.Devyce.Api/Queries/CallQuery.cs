namespace ApiBureau.Devyce.Api.Queries;

public sealed record CallQuery(
    DateTime? StartDateInclusive,
    DateTime? EndDateExclusive,
    int? Limit = null,
    string? OwnerId = null,
    string? ContinuationToken = null);
