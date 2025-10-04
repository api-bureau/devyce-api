using Microsoft.Extensions.Logging;
using System.Text;

namespace ApiBureau.Devyce.Api.Core;

/// <summary>
/// Validates Devyce client configuration prior to making HTTP requests.
/// </summary>
/// <remarks>
/// Use this validator during startup to fail fast when required settings are missing.
/// </remarks>
public static class DevyceValidator
{
    private const string MissingSettings = "Settings are missing in the appsettings.json or secret.json";
    private const string ApiKeyNotFound = "API Key in settings is missing or empty";
    private const string BaseUrlNotFound = "Base URL in settings is missing or empty";

    /// <summary>
    /// Validates the provided <see cref="DevyceSettings"/> instance.
    /// </summary>
    /// <param name="settings">Settings to validate.</param>
    /// <param name="logger">Logger used to record validation failures.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="settings"/> is <c>null</c>.</exception>
    /// <exception cref="ArgumentException">Thrown when required properties are empty or invalid.</exception>
    public static void ValidateSettings(DevyceSettings settings, ILogger logger)
    {
        if (settings is null)
        {
            logger.LogError(MissingSettings);

            throw new ArgumentNullException(nameof(settings), MissingSettings);
        }

        var errors = new StringBuilder();

        if (string.IsNullOrWhiteSpace(settings.BaseUrl))
            errors.AppendLine(BaseUrlNotFound);

        if (string.IsNullOrWhiteSpace(settings.ApiKey))
            errors.AppendLine(ApiKeyNotFound);


        if (errors.Length > 0)
        {
            var errorMessage = errors.ToString().TrimEnd();

            logger.LogError("Settings validation errors: {errors}", errorMessage);

            throw new ArgumentException(errorMessage, nameof(settings));
        }
    }
}