using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net.Http.Json;
using System.Text.Json;

namespace ApiBureau.Devyce.Api.Http;

/// <summary>
/// Lightweight HTTP client wrapper for the Devyce API.
/// </summary>
/// <remarks>
/// Responsibilities:
/// - Assumes the underlying <see cref="HttpClient"/> is configured via DI (BaseAddress and Basic auth).
/// - Provides minimal JSON-based GET helper with optional cancellation support.
/// - Centralizes error logging for deserialization failures.
/// </remarks>
public class DevyceHttpClient
{
    private const string ApiUrlPrefix = "/Organizations";

    private static class KnownHeaderNames
    {
        public const string CompanyId = "companyId";
    }

    private readonly HttpClient _client;
    private readonly ILogger<DevyceHttpClient> _logger;
    private readonly DevyceSettings _settings;
    private readonly JsonSerializerOptions _jsonOptions;

    /// <summary>
    /// Creates a new instance of <see cref="DevyceHttpClient"/> configured for the Devyce API.
    /// </summary>
    /// <param name="httpClient">The <see cref="HttpClient"/> to use for requests. Configured in DI.</param>
    /// <param name="settings">Typed Devyce settings (BaseUrl and ApiKey) provided via options.</param>
    /// <param name="logger">Logger for diagnostics and error reporting.</param>
    public DevyceHttpClient(HttpClient httpClient, IOptions<DevyceSettings> settings, ILogger<DevyceHttpClient> logger)
    {
        _client = httpClient;
        _logger = logger;
        _settings = settings.Value;

        // Settings are validated in DI. Keep this to log obvious misconfigurations early.
        DevyceValidator.ValidateSettings(_settings, _logger);

        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        _jsonOptions.Converters.Add(new Converters.CustomDateTimeConverter("MM/dd/yyyy"));
    }

    /// <summary>
    /// Issues a GET request to the specified Devyce relative URL and deserializes the JSON response.
    /// </summary>
    /// <typeparam name="T">The expected response type.</typeparam>
    /// <param name="url">Relative resource path under the Devyce API prefix.</param>
    /// <returns>The deserialized response instance, or <c>null</c> if deserialization fails.</returns>
    public Task<T?> GetAsync<T>(string url)
        => GetAsync<T>(url, CancellationToken.None);

    /// <summary>
    /// Issues a GET request to the specified Devyce relative URL and deserializes the JSON response.
    /// </summary>
    /// <typeparam name="T">The expected response type.</typeparam>
    /// <param name="url">Relative resource path under the Devyce API prefix.</param>
    /// <param name="token">A token to observe while waiting for the task to complete.</param>
    /// <returns>The deserialized response instance, or <c>null</c> if deserialization fails.</returns>
    public Task<T?> GetAsync<T>(string url, CancellationToken token)
        => GetAsync<T>(url, (IDictionary<string, string>?)null, token);

    /// <summary>
    /// Issues a GET request to the specified Devyce relative URL and deserializes the JSON response.
    /// Allows supplying request-specific headers (e.g. company id).
    /// </summary>
    /// <param name="url">Relative resource path under the Devyce API prefix.</param>
    /// <param name="headers">Dictionary of headers to add only to this request.</param>
    /// <param name="token">Cancellation token.</param>
    public async Task<T?> GetAsync<T>(string url, IDictionary<string, string>? headers, CancellationToken token)
    {
        try
        {
            var relative = Combine(ApiUrlPrefix + $"/{_settings.OrganizationId}", url);

            // Fast path for no headers: use convenience extension
            if (headers == null || headers.Count == 0)
            {
                return await _client.GetFromJsonAsync<T>(relative, _jsonOptions, token).ConfigureAwait(false);
            }

            using var request = new HttpRequestMessage(HttpMethod.Get, relative);

            // Add headers only to this request; do not touch DefaultRequestHeaders
            foreach (var kv in headers)
            {
                // TryAddWithoutValidation prevents header validation exceptions for custom names
                request.Headers.TryAddWithoutValidation(kv.Key, kv.Value);
            }

            using var response = await _client.SendAsync(request, token).ConfigureAwait(false);

            // Let non-success throw as before (caller can catch if needed)
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<T>(_jsonOptions, cancellationToken: token).ConfigureAwait(false);
        }
        catch (JsonException e)
        {
            _logger.LogError(e, "Failed to deserialize response from Devyce API. URL: {Url}", url);

            return default;
        }
        catch (Exception)
        {
            throw;
        }
    }

    /// <summary>
    /// Convenience overload for endpoints that require a companyId in a header.
    /// Uses the header name "companyId" by default — change if your API expects a different header.
    /// </summary>
    public Task<T?> GetAsync<T>(string url, string? companyId, CancellationToken token)
    {
        if (string.IsNullOrWhiteSpace(companyId))
            return GetAsync<T>(url, token);

        var headers = new Dictionary<string, string>(1)
        {
            [KnownHeaderNames.CompanyId] = companyId
        };

        return GetAsync<T>(url, headers, token);
    }

    /// <summary>
    /// Combines a prefix and a path into a single, well-formed URL or file path segment.
    /// </summary>
    /// <remarks>If <paramref name="path"/> is null, empty, or consists only of whitespace, the method returns
    /// <paramref name="prefix"/> unchanged.</remarks>
    /// <param name="prefix">The prefix to use as the base. This is typically a URL or directory path.</param>
    /// <param name="path">The path to append to the prefix. Leading and trailing slashes will be normalized.</param>
    /// <returns>A combined string with the prefix and path, ensuring there is exactly one slash between them.</returns>
    private static string Combine(string prefix, string path)
    {
        if (string.IsNullOrWhiteSpace(path)) return prefix;

        return $"{prefix.TrimEnd('/')}/{path.TrimStart('/')}";
    }
}