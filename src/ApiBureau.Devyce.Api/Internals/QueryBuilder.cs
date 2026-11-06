using Microsoft.AspNetCore.WebUtilities;
using System.Globalization;

namespace ApiBureau.Devyce.Api.Internals;

internal static class QueryBuilder
{
    internal static string BuildCallQuery(string resourcePath, CallQuery query)
    {
        var queryParams = CreatePagedQuery(query.StartDateInclusive, query.EndDateExclusive, query.Limit, query.ContinuationToken);

        if (!string.IsNullOrEmpty(query.OwnerId))
        {
            queryParams["ownerId"] = query.OwnerId;
        }

        return QueryHelpers.AddQueryString(resourcePath, queryParams);
    }

    internal static string BuildMeetingQuery(string resourcePath, MeetingQuery query)
    {
        var queryParams = CreatePagedQuery(query.StartDateInclusive, query.EndDateExclusive, query.Limit, query.ContinuationToken);

        return QueryHelpers.AddQueryString(resourcePath, queryParams);
    }

    private static Dictionary<string, string?> CreatePagedQuery(DateTime? startDateInclusive, DateTime? endDateExclusive, int? limit, string? continuationToken)
    {
        var queryParams = new Dictionary<string, string?>();

        if (startDateInclusive.HasValue)
        {
            queryParams["startDateInclusive"] = startDateInclusive.Value.ToString("o", CultureInfo.InvariantCulture);
        }

        if (endDateExclusive.HasValue)
        {
            queryParams["endDateExclusive"] = endDateExclusive.Value.ToString("o", CultureInfo.InvariantCulture);
        }

        if (limit.HasValue)
        {
            queryParams["limit"] = limit.Value.ToString(CultureInfo.InvariantCulture);
        }

        if (!string.IsNullOrEmpty(continuationToken))
        {
            queryParams["continuationToken"] = continuationToken;
        }

        return queryParams;
    }
}