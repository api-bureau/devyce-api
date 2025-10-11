using Microsoft.AspNetCore.WebUtilities;
using System.Globalization;

namespace ApiBureau.Devyce.Api.Internals;

internal static class QueryBuilder
{
    internal static string BuildCallQuery(CallQuery query)
    {
        var queryParams = new Dictionary<string, string?>();
        if (query.StartDateInclusive.HasValue)
        {
            queryParams["startDateInclusive"] = query.StartDateInclusive.Value.ToString("o", CultureInfo.InvariantCulture);
        }

        if (query.EndDateExclusive.HasValue)
        {
            queryParams["endDateExclusive"] = query.EndDateExclusive.Value.ToString("o", CultureInfo.InvariantCulture);
        }

        if (query.Limit.HasValue)
        {
            queryParams["limit"] = query.Limit.Value.ToString(CultureInfo.InvariantCulture);
        }

        if (!string.IsNullOrEmpty(query.OwnerId))
        {
            queryParams["ownerId"] = query.OwnerId;
        }

        if (!string.IsNullOrEmpty(query.ContinuationToken))
        {
            queryParams["continuationToken"] = query.ContinuationToken;
        }

        return QueryHelpers.AddQueryString($"/Calls", queryParams);
    }
}