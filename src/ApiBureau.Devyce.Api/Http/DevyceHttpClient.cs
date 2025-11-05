using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net;
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
/// - Handles 204 No Content responses gracefully by returning default values.
/// </remarks>
public class DevyceHttpClient
{
    private const string ApiUrlPrefix = "/Organizations";
    private const string CompanyIdHeaderName = "companyId";

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
    /// <returns>The deserialized response instance, or <c>null</c> if deserialization fails or 204 No Content is returned.</returns>
    public Task<T?> GetAsync<T>(string url)
        => GetAsync<T>(url, CancellationToken.None);

    /// <summary>
    /// Issues a GET request to the specified Devyce relative URL and deserializes the JSON response.
    /// </summary>
    /// <typeparam name="T">The expected response type.</typeparam>
    /// <param name="url">Relative resource path under the Devyce API prefix.</param>
    /// <param name="token">A token to observe while waiting for the task to complete.</param>
    /// <returns>The deserialized response instance, or <c>null</c> if deserialization fails or 204 No Content is returned.</returns>
    public Task<T?> GetAsync<T>(string url, CancellationToken token)
        => GetAsync<T>(url, headers: null, token);

    /// <summary>
    /// Issues a GET request to the specified Devyce relative URL and deserializes the JSON response.
    /// Allows supplying request-specific headers (e.g. company id).
    /// </summary>
    /// <typeparam name="T">The expected response type.</typeparam>
    /// <param name="url">Relative resource path under the Devyce API prefix.</param>
    /// <param name="headers">Dictionary of headers to add only to this request.</param>
    /// <param name="token">Cancellation token.</param>
    /// <returns>The deserialized response instance, or <c>null</c> if deserialization fails or 204 No Content is returned.</returns>
    public async Task<T?> GetAsync<T>(string url, IDictionary<string, string>? headers, CancellationToken token)
    {
        try
        {
            var relative = BuildRelativeUrl(url);

            using var request = new HttpRequestMessage(HttpMethod.Get, relative);

            // Add request-specific headers without touching DefaultRequestHeaders
            if (headers is not null)
            {
                foreach (var (key, value) in headers)
                {
                    request.Headers.TryAddWithoutValidation(key, value);
                }
            }

            using var response = await _client.SendAsync(request, token).ConfigureAwait(false);

            if (response.StatusCode == HttpStatusCode.NoContent)
            {
                _logger.LogDebug("Received 204 No Content for {Url}", url);

                return default;
            }

            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<T>(_jsonOptions, cancellationToken: token).ConfigureAwait(false);
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Failed to deserialize response from Devyce API. URL: {Url}", url);

            return default;
        }
    }

    /// <summary>
    /// Convenience overload for endpoints that require a companyId in a header.
    /// Uses the header name "companyId" by default.
    /// </summary>
    /// <typeparam name="T">The expected response type.</typeparam>
    /// <param name="url">Relative resource path under the Devyce API prefix.</param>
    /// <param name="companyId">The company identifier to include in the request header.</param>
    /// <param name="token">Cancellation token.</param>
    /// <returns>The deserialized response instance, or <c>null</c> if deserialization fails or 204 No Content is returned.</returns>
    public Task<T?> GetAsync<T>(string url, string? companyId, CancellationToken token)
    {
        if (string.IsNullOrWhiteSpace(companyId))
        {
            return GetAsync<T>(url, token);
        }

        var headers = new Dictionary<string, string>(capacity: 1)
        {
            [CompanyIdHeaderName] = companyId
        };

        return GetAsync<T>(url, headers, token);
    }

    /// <summary>
    /// Builds the relative URL by combining the organization-specific prefix with the requested path.
    /// </summary>
    /// <param name="path">The resource path to append.</param>
    /// <returns>A well-formed relative URL string.</returns>
    private string BuildRelativeUrl(string path)
    {
        var orgPrefix = $"{ApiUrlPrefix}/{_settings.OrganizationId}";

        if (string.IsNullOrWhiteSpace(path)) return orgPrefix;

        return $"{orgPrefix.TrimEnd('/')}/{path.TrimStart('/')}";
    }
}